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


        public async Task<bool> AssignRoomToAppointmentAsync(string appointmentId, DateTime preparationStart, DateTime preparationEnd, DateTime surgeryStart, DateTime surgeryEnd, DateTime cleaningStart, DateTime cleaningEnd,TimeSpan duration)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BackOfficeDbContext>();

                // Find a room that is free during the entire required time span (from preparation to end of cleaning)
                var availableRoom = await dbContext.SurgeryRoom
                    .Include(r => r.Phases)
                    .Include(r => r.MaintenanceSlots)
                    .Where(r => !r.Phases.Any(phase =>
                            (preparationStart < phase.EndTime && preparationEnd > phase.StartTime) || // Overlaps preparation
                            (surgeryStart < phase.EndTime && surgeryEnd > phase.StartTime) ||         // Overlaps surgery
                            (cleaningStart < phase.EndTime && cleaningEnd > phase.StartTime))         // Overlaps cleaning
                        && !r.MaintenanceSlots.Any(slot =>
                            (preparationStart < slot.End && cleaningEnd > slot.Start)))               // Overlaps maintenance
                    .FirstOrDefaultAsync();

                if (availableRoom == null)
                {
                    Console.WriteLine("No available surgery room found for the specified time slots.");
                    return false;
                }

                // Assign each phase to the room with the specified appointment ID
                var preparationPhase = new SurgeryPhaseDataModel
                {
                    RoomNumber = availableRoom.RoomNumber,
                    PhaseType = "Preparation",
                    Duration = new TimeSpan(0,30,0),
                    StartTime = preparationStart,
                    EndTime = preparationEnd,
                    AppointementId = appointmentId
                };

                var surgeryPhase = new SurgeryPhaseDataModel
                {
                    RoomNumber = availableRoom.RoomNumber,
                    PhaseType = "Surgery",
                    Duration = duration,
                    StartTime = surgeryStart,
                    EndTime = surgeryEnd,
                    AppointementId = appointmentId
                };

                var cleaningPhase = new SurgeryPhaseDataModel
                {
                    RoomNumber = availableRoom.RoomNumber,
                    PhaseType = "Cleaning",
                    Duration = new TimeSpan(0,45,0),
                    StartTime = cleaningStart,
                    EndTime = cleaningEnd,
                    AppointementId = appointmentId
                };

                // Add the phases to the context
                await dbContext.SurgeryPhaseDataModel.AddRangeAsync(preparationPhase, surgeryPhase, cleaningPhase);

                // Save changes
                await dbContext.SaveChangesAsync();

                Console.WriteLine($"Surgery room {availableRoom.RoomNumber} assigned successfully for appointment {appointmentId}.");
                return true;
            }
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
