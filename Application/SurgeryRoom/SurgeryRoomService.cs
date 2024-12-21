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


    public async Task<(List<(DateTime Start, DateTime End)> AvailableSlots, List<(string Specialization, int NeededPersonnel)> Requirements, Dictionary<string, List<string>> StaffBySpecialization)> GetAvailableTimeSlotsAsync(DateTime date, string requestId, string roomNumber)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BackOfficeDbContext>();

                // Define the working hours
                DateTime startOfDay = date.Date.AddHours(8).AddMinutes(30); // 08:30
                DateTime endOfDay = date.Date.AddHours(21); // 21:00

                var room = await dbContext.SurgeryRoom
                    .Include(r => r.Phases)
                    .Include(r => r.MaintenanceSlots)
                    .FirstOrDefaultAsync(r => r.RoomNumber == roomNumber);

                if (room == null)
                {
                    throw new ArgumentException($"Room with number {roomNumber} does not exist.");
                }

                // Fetch the OperationRequest and related OperationType
                var operationRequest = await dbContext.OperationRequests
                    .FirstOrDefaultAsync(or => or.RequestId == new Guid(requestId));

                if (operationRequest == null || operationRequest.OperationType == null)
                {
                    throw new ArgumentException($"OperationRequest with ID {requestId} does not exist or has no associated OperationType.");
                }

                var operationType = await dbContext.OperationType
                    .FirstOrDefaultAsync(ot => ot.OperationTypeName == operationRequest.OperationType);

                if (operationType == null)
                {
                    throw new ArgumentException($"OperationType {operationRequest.OperationType} does not exist.");
                }

                // Calculate total operation duration
                int totalOperationDurationInMinutes = operationType.PreparationTime +
                                                        operationType.SurgeryTime +
                                                        operationType.CleaningTime;

                // Retrieve the requirements for the OperationType
                var operationRequirements = await dbContext.OperationRequirements
                    .Where(or => or.OperationTypeId == operationType.OperationTypeId)
                    .Select(or => new { or.Name, or.NeededPersonnel })
                    .ToListAsync();

                var requirements = operationRequirements
                    .Select(or => (Specialization: or.Name, NeededPersonnel: or.NeededPersonnel))
                    .ToList();

                // Get staff by specialization
                var specializations = requirements.Select(r => r.Specialization).ToList();
                var staffBySpecialization = await GetStaffBySpecializationAsync(specializations);

                // Calculate available time slots
                var allAvailableSlots = new List<(DateTime Start, DateTime End)>();

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
                    while (currentStart.AddMinutes(totalOperationDurationInMinutes) <= slot.Start)
                    {
                        var newSlot = (currentStart, currentStart.AddMinutes(totalOperationDurationInMinutes));
                        allAvailableSlots.Add(newSlot);

                        currentStart = currentStart.AddMinutes(totalOperationDurationInMinutes);
                    }

                    if (currentStart < slot.End)
                    {
                        currentStart = slot.End;
                    }
                }

                while (currentStart.AddMinutes(totalOperationDurationInMinutes) <= endOfDay)
                {
                    var newSlot = (currentStart, currentStart.AddMinutes(totalOperationDurationInMinutes));
                    allAvailableSlots.Add(newSlot);
                    currentStart = currentStart.AddMinutes(totalOperationDurationInMinutes);
                }

                return (allAvailableSlots.OrderBy(slot => slot.Start).ToList(), requirements, staffBySpecialization);
            }
        }

        public async Task<Dictionary<string, List<string>>> GetStaffBySpecializationAsync(List<string> specializations)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BackOfficeDbContext>();

                var staffBySpecialization = await dbContext.Staff
                    .Where(s => specializations.Contains(s.Specialization) && s.Status == true) // Assuming status 1 means active
                    .GroupBy(s => s.Specialization)
                    .ToDictionaryAsync(g => g.Key, g => g.Select(s => s.StaffId).ToList());

                return staffBySpecialization;
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
