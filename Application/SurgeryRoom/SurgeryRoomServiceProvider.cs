using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BackOffice.Application.SurgeryRoom;
using BackOffice.Infrastructure;
using Healthcare.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Healthcare.Domain.Services
{
    public class SurgeryRoomServiceProvider
    {
        private BackOfficeDbContext _dbContext;

        public SurgeryRoomServiceProvider(BackOfficeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<int>> GetAllSurgeryRoomsAsync()
        {
            // Fetch surgery rooms and map their statuses to a list of integers
            var currentStatus = await _dbContext.SurgeryRoom
                .OrderBy(room => room.RoomNumber) // Ensure the list is in order of RoomNumber
                .Select(room => room.CurrentStatus == "Occupied" ? 1 : 0) // Map status to 1 or 0
                .ToListAsync();

            return currentStatus;
        }

        public async Task<SurgeryRoomDto> AddAsync(SurgeryRoomDto surgeryRoomDto)
        {
            var existingRoom = await _dbContext.SurgeryRoom
                .FirstOrDefaultAsync(sr => sr.RoomNumber == surgeryRoomDto.RoomNumber);

            if (existingRoom != null)
            {
                throw new ArgumentException("A surgery room with the same RoomNumber already exists.");
            }

            var newSurgeryRoom = new SurgeryRoomDataModel
            {
                RoomNumber = surgeryRoomDto.RoomNumber,
                Type = surgeryRoomDto.Type,
                Capacity = surgeryRoomDto.Capacity,
                CurrentStatus = "Available"
            };

            await _dbContext.SurgeryRoom.AddAsync(newSurgeryRoom);
            await _dbContext.SaveChangesAsync();

            return new SurgeryRoomDto(
                newSurgeryRoom.RoomNumber,
                newSurgeryRoom.Type,
                newSurgeryRoom.Capacity
            );
        }


    }
}
