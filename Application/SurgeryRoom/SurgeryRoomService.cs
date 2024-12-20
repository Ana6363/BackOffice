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


        public async Task<List<(DateTime Start, DateTime End)>> GetAvailableTimeSlotsAsync(DateTime date, int operationDurationInMinutes)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BackOfficeDbContext>();

                // Define the working hours
                DateTime startOfDay = date.Date.AddHours(8).AddMinutes(30); // 08:30
                DateTime endOfDay = date.Date.AddHours(21); // 21:00

                var rooms = await dbContext.SurgeryRoom
                    .Include(r => r.Phases)
                    .Include(r => r.MaintenanceSlots)
                    .ToListAsync();

                var allAvailableSlots = new List<(DateTime Start, DateTime End)>();

                foreach (var room in rooms)
                {
                    var occupiedSlots = room.Phases
                        .Where(phase => phase.StartTime.Date == date.Date)
                        .Select(phase => (Start: phase.StartTime, End: phase.EndTime))
                        .Concat(room.MaintenanceSlots
                            .Where(slot => slot.Start.Date == date.Date)
                            .Select(slot => (Start: slot.Start, End: slot.End)))
                        .OrderBy(slot => slot.Start)
                        .ToList();

                    DateTime currentStart = startOfDay;

                    foreach (var slot in occupiedSlots)
                    {
                        while (currentStart.AddMinutes(operationDurationInMinutes) <= slot.Start)
                        {
                            var newSlot = (currentStart, currentStart.AddMinutes(operationDurationInMinutes));
                            allAvailableSlots.Add(newSlot);

                            currentStart = currentStart.AddMinutes(operationDurationInMinutes);
                        }

                        if (currentStart < slot.End)
                        {
                            currentStart = slot.End;
                        }
                    }
                    while (currentStart.AddMinutes(operationDurationInMinutes) <= endOfDay)
                    {
                        var newSlot = (currentStart, currentStart.AddMinutes(operationDurationInMinutes));
                        allAvailableSlots.Add(newSlot);
                        currentStart = currentStart.AddMinutes(operationDurationInMinutes);
                    }
                }

                var distinctTimeSlots = allAvailableSlots
                    .Distinct(new TimeSlotComparer())
                    .OrderBy(slot => slot.Start)
                    .ToList();

                return distinctTimeSlots;
            }
        }



        public async Task<string> GetAvailableRoomAsync(DateTime startTime, DateTime endTime)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BackOfficeDbContext>();

                var availableRoom = await dbContext.SurgeryRoom
                    .Include(r => r.Phases)
                    .Include(r => r.MaintenanceSlots)
                    .FirstOrDefaultAsync(room =>
                        !room.Phases.Any(phase => phase.StartTime < endTime && phase.EndTime > startTime) &&
                        !room.MaintenanceSlots.Any(slot => slot.Start < endTime && slot.End > startTime));

                if (availableRoom == null)
                    throw new Exception("No available surgery rooms for the specified time.");

                return availableRoom.RoomNumber;
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

    public class TimeSlotComparer : IEqualityComparer<(DateTime Start, DateTime End)>
        {
            public bool Equals((DateTime Start, DateTime End) x, (DateTime Start, DateTime End) y)
            {
                return x.Start == y.Start && x.End == y.End;
            }

            public int GetHashCode((DateTime Start, DateTime End) obj)
            {
                return HashCode.Combine(obj.Start, obj.End);
            }
        }

}
