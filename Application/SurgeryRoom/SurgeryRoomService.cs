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
    public class SurgeryRoomService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        public SurgeryRoomService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(CheckRoomStatus, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private void CheckRoomStatus(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BackOfficeDbContext>();
                var currentTime = DateTime.Now;

                var surgeryRooms = dbContext.SurgeryRoom
                    .Include(r => r.Phases)
                    .Include(r => r.MaintenanceSlots)
                    .ToList();

                foreach (var room in surgeryRooms)
                {
                    bool isOccupied = room.Phases.Any(phase =>
                        phase.StartTime <= currentTime && phase.EndTime >= currentTime);

                    bool isUnderMaintenance = room.MaintenanceSlots.Any(slot =>
                        slot.Start <= currentTime && slot.End >= currentTime);

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

                    dbContext.SurgeryRoom.Update(room);
                }

                dbContext.SaveChanges();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
