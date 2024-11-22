using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BackOffice.Infrastructure;
using Healthcare.Domain.Enums;
using Healthcare.Domain.Services;

namespace Healthcare.Api.Controllers
{
    [ApiController]
    [Route("api/surgeryRoom")]
    public class SurgeryRoomController : ControllerBase
    {
        private readonly BackOfficeDbContext _dbContext;
        private readonly SurgeryRoomServiceProvider _surgeryRoomServiceProvider;

        public SurgeryRoomController(BackOfficeDbContext dbContext,SurgeryRoomServiceProvider surgeryRoomServiceProvider)
        {
            _dbContext = dbContext;
            _surgeryRoomServiceProvider = surgeryRoomServiceProvider;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSurgeryRooms()
        {
            var rooms = await _surgeryRoomServiceProvider.GetAllSurgeryRoomsAsync();
            return Ok(rooms);
        }

        // GET: api/SurgeryRoom/{roomNumber}
        [HttpGet("{roomNumber}")]
        public async Task<IActionResult> GetSurgeryRoomByRoomNumber(string roomNumber)
        {
            var room = await _dbContext.SurgeryRoom
                .Include(r => r.MaintenanceSlots)
                .Include(r => r.Equipments)
                .Include(r => r.Phases)
                .Where(r => r.RoomNumber == roomNumber)
                .Select(room => new
                {
                    RoomNumber = room.RoomNumber,
                    Type = room.Type,
                    Capacity = room.Capacity,
                    CurrentStatus = room.CurrentStatus,
                    IsUnderMaintenance = room.MaintenanceSlots.Any(slot =>
                        slot.Start <= DateTime.Now && slot.End >= DateTime.Now),
                    AssignedEquipment = room.Equipments.Select(eq => eq.EquipmentName).ToList(),
                    Phases = room.Phases.Select(p => new
                    {
                        PhaseType = p.PhaseType,
                        Duration = p.Duration,
                        StartTime = p.StartTime,
                        EndTime = p.EndTime,
                        AppointmentId = p.AppointementId
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (room == null)
                return NotFound($"Surgery room with RoomNumber {roomNumber} not found.");

            return Ok(room);
        }

        // POST: api/SurgeryRoom
        [HttpPost("create")]
        public async Task<IActionResult> CreateSurgeryRoom([FromBody] SurgeryRoomDataModel newRoom)
        {
            if (await _dbContext.SurgeryRoom.AnyAsync(r => r.RoomNumber == newRoom.RoomNumber))
                return Conflict($"A surgery room with RoomNumber {newRoom.RoomNumber} already exists.");

            _dbContext.SurgeryRoom.Add(newRoom);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSurgeryRoomByRoomNumber), new { roomNumber = newRoom.RoomNumber }, newRoom);
        }

        // PUT: api/SurgeryRoom/{roomNumber}
        [HttpPut("update")]
        public async Task<IActionResult> UpdateSurgeryRoom(string roomNumber, [FromBody] SurgeryRoomDataModel updatedRoom)
        {
            var existingRoom = await _dbContext.SurgeryRoom.FindAsync(roomNumber);
            if (existingRoom == null)
                return NotFound($"Surgery room with RoomNumber {roomNumber} not found.");

            existingRoom.Type = updatedRoom.Type;
            existingRoom.Capacity = updatedRoom.Capacity;
            existingRoom.CurrentStatus = updatedRoom.CurrentStatus;

            _dbContext.SurgeryRoom.Update(existingRoom);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/SurgeryRoom/{roomNumber}
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteSurgeryRoom(string roomNumber)
        {
            var room = await _dbContext.SurgeryRoom.FindAsync(roomNumber);
            if (room == null)
                return NotFound($"Surgery room with RoomNumber {roomNumber} not found.");

            _dbContext.SurgeryRoom.Remove(room);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/SurgeryRoom/refresh
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshRoomStatus()
        {
            var currentTime = DateTime.Now;

            var surgeryRooms = await _dbContext.SurgeryRoom
                .Include(r => r.MaintenanceSlots)
                .Include(r => r.Phases)
                .ToListAsync();

            foreach (var room in surgeryRooms)
            {
                bool isUnderMaintenance = room.MaintenanceSlots.Any(slot =>
                    slot.Start <= currentTime && slot.End >= currentTime);

                bool isOccupied = room.Phases.Any(phase =>
                    phase.StartTime <= currentTime && phase.EndTime >= currentTime);

                if (isUnderMaintenance)
                {
                    room.CurrentStatus = RoomStatus.UnderMaintenance.ToString();
                }
                else if (isOccupied)
                {
                    room.CurrentStatus = RoomStatus.Occupied.ToString();
                }
                else
                {
                    room.CurrentStatus = RoomStatus.Available.ToString();
                }
            }

            _dbContext.SurgeryRoom.UpdateRange(surgeryRooms);
            await _dbContext.SaveChangesAsync();

            return Ok("Room statuses have been refreshed.");
        }
    }
}
