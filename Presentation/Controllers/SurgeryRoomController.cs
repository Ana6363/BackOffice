using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BackOffice.Infrastructure;
using Healthcare.Domain.Enums;
using Healthcare.Domain.Services;
using BackOffice.Application.SurgeryRoom;

namespace Healthcare.Api.Controllers
{
    [ApiController]
    [Route("api/v1/surgeryRoom")]
    public class SurgeryRoomController : ControllerBase
    {
        private readonly BackOfficeDbContext _dbContext;
        private readonly SurgeryRoomServiceProvider _surgeryRoomServiceProvider;
        private readonly SurgeryRoomService _surgeryRoomService;

        public SurgeryRoomController(BackOfficeDbContext dbContext,SurgeryRoomServiceProvider surgeryRoomServiceProvider, SurgeryRoomService surgeryRoomService)
        {
            _dbContext = dbContext;
            _surgeryRoomServiceProvider = surgeryRoomServiceProvider;
            _surgeryRoomService = surgeryRoomService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSurgeryRooms()
        {
            var rooms = await _surgeryRoomServiceProvider.GetAllSurgeryRoomsAsync();
            return Ok(rooms);
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetSurgeryRooms()
        {
            var rooms = await _dbContext.SurgeryRoom
                .Select(room => new
                {
                    RoomNumber = room.RoomNumber,
                    Type = room.Type,
                    Capacity = room.Capacity,
                    CurrentStatus = room.CurrentStatus
                })
                .ToListAsync();

            if (rooms == null || !rooms.Any())
                return NotFound("No surgery rooms found.");

            return Ok(rooms);
        }


        // POST: api/SurgeryRoom
        [HttpPost("create")]
        public async Task<IActionResult> CreateSurgeryRoom([FromBody] SurgeryRoomDto newRoom)
        {
            if (newRoom == null)
            {
                return BadRequest(new { success = false, message = "Room details are required." });
            }

            try
            {
                // Call the AddAsync method to add the room
                var createdRoom = await _surgeryRoomServiceProvider.AddAsync(newRoom);
                return Ok(new { success = true, message = "Surgery room created successfully.", data = createdRoom });
            }
            catch (ArgumentException ex)
            {
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while creating the surgery room.", details = ex.Message });
            }
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
        
        [HttpGet("getDailyTimeSlots")]
        public async Task<IActionResult> GetDailyTimeSlots([FromQuery] DateTime date, [FromQuery] string requestId, [FromQuery] string roomNumber)
        {
            try
            {
                var (availableTimeSlots, requirements, staffBySpecialization) = await _surgeryRoomService.GetAvailableTimeSlotsAsync(date, requestId, roomNumber);

                if (!availableTimeSlots.Any())
                {
                    return NotFound("No available time slots found for the specified date and duration.");
                }

                return Ok(new
                {
                    TimeSlots = availableTimeSlots.Select(slot => new
                    {
                        Start = slot.Start.ToString("yyyy-MM-dd HH:mm"),
                        End = slot.End.ToString("yyyy-MM-dd HH:mm")
                    }),
                    Requirements = requirements.Select(req => new
                    {
                        Specialization = req.Specialization,
                        NeededPersonnel = req.NeededPersonnel
                    }),
                    StaffBySpecialization = staffBySpecialization.Select(kvp => new
                    {
                        Specialization = kvp.Key,
                        StaffIds = kvp.Value
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while fetching time slots: {ex.Message}");
            }
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
