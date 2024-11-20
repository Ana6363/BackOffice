using Healthcare.Domain.Services;
using BackOffice.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace BackOffice.Controllers
{
    [ApiController]
    [Route("api/v1/surgeryRoom")]
    public class SurgeryRoomController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly SurgeryRoomService _surgeryRoomService;

        public SurgeryRoomController(IServiceProvider serviceProvider, SurgeryRoomService surgeryRoomService)
        {
            _serviceProvider = serviceProvider;
            _surgeryRoomService = surgeryRoomService;
        }

        /// <summary>
        /// Verifica a disponibilidade de uma sala de cirurgia para um intervalo de tempo.
        /// </summary>
        [HttpGet("check-availability")]
        public async Task<IActionResult> CheckRoomAvailabilityAsync(DateTime preparationStart, DateTime cleaningEnd)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BackOfficeDbContext>();

                var availableRoom = await dbContext.SurgeryRoom
                    .Include(r => r.Phases)
                    .Include(r => r.MaintenanceSlots)
                    .FirstOrDefaultAsync(r =>
                        !r.Phases.Any(phase =>
                            (preparationStart < phase.EndTime && cleaningEnd > phase.StartTime)) &&
                        !r.MaintenanceSlots.Any(slot =>
                            (preparationStart < slot.End && cleaningEnd > slot.Start)));

                if (availableRoom != null)
                {
                    return Ok(new
                    {
                        success = true,
                        message = $"Room {availableRoom.RoomNumber} is available.",
                        room = availableRoom
                    });
                }

                return BadRequest(new
                {
                    success = false,
                    message = "No rooms are available for the specified time range."
                });
            }
        }

        /// <summary>
        /// Atribui uma sala de cirurgia para um agendamento específico.
        /// </summary>
        [HttpPost("assign-room")]
        public async Task<IActionResult> AssignRoomToAppointmentAsync(
            string appointmentId,
            DateTime preparationStart,
            DateTime preparationEnd,
            DateTime surgeryStart,
            DateTime surgeryEnd,
            DateTime cleaningStart,
            DateTime cleaningEnd)
        {
            try
            {
                var isAssigned = await _surgeryRoomService.AssignRoomToAppointmentAsync(
                    appointmentId,
                    preparationStart,
                    preparationEnd,
                    surgeryStart,
                    surgeryEnd,
                    cleaningStart,
                    cleaningEnd,
                    surgeryEnd - surgeryStart
                );

                if (isAssigned)
                {
                    return Ok(new { success = true, message = "Room successfully assigned to the appointment." });
                }

                return BadRequest(new { success = false, message = "No rooms available for the specified time slots." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Obtém o estado atual de todas as salas de cirurgia.
        /// </summary>
        [HttpGet("status")]
        public async Task<IActionResult> GetRoomStatusAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BackOfficeDbContext>();

                var rooms = await dbContext.SurgeryRoom
                    .Include(r => r.Phases)
                    .Include(r => r.MaintenanceSlots)
                    .ToListAsync();

                var roomStatuses = rooms.Select(room => new
                {
                    room.RoomNumber,
                    room.CurrentStatus,
                    isOccupied = room.Phases.Any(phase =>
                        phase.StartTime <= DateTime.Now && phase.EndTime >= DateTime.Now),
                    isUnderMaintenance = room.MaintenanceSlots.Any(slot =>
                        slot.Start <= DateTime.Now && slot.End >= DateTime.Now)
                });

                return Ok(new { success = true, rooms = roomStatuses });
            }
        }
    }
}
