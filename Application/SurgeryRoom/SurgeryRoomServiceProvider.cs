using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public async Task<object> GetAllSurgeryRoomsAsync()
        {
            // Fetch surgery rooms directly from the database
            var rooms = await _dbContext.SurgeryRoom
                .Select(room => new
                {
                    RoomNumber = room.RoomNumber,
                    Type = room.Type,
                    Capacity = room.Capacity,
                    CurrentStatus = room.CurrentStatus
                })
                .ToListAsync();

            return rooms;
        }

    }
}
