using Microsoft.EntityFrameworkCore; 
using Pomelo.EntityFrameworkCore.MySql.Infrastructure; 
using BackOffice.Domain.Users; 
using BackOffice.Infrastructure.Users; 
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using BackOffice.Infrastructure.Persistence.Models;
using BackOffice.Infrastructure.Patients;
using BackOffice.Domain.Patients;
using BackOffice.Infrastructure.Staff;
using BackOffice.Domain.OperationRequest;
using BackOffice.Infraestructure.OperationRequest;
using BackOffice.Infraestructure.Appointement;
using BackOffice.Infrastructure.OperationTypes;
using BackOffice.Infraestructure.NeededPersonnel;
using BackOffice.Infraestructure.Specialization;

namespace BackOffice.Infrastructure
{
    public class BackOfficeDbContext : DbContext
    {
        public DbSet<UserDataModel> Users { get; set; } 
        public DbSet<PatientDataModel> Patients { get; set; }
        public DbSet<LogDataModel> Logs { get; set; }
        public DbSet<StaffDataModel> Staff { get; set; }
        public DbSet<AvailableSlotDataModel> AvailableSlots { get; set; }
        public DbSet<OperationRequestDataModel> OperationRequests { get; set; }
        public DbSet<AppointementDataModel> Appointements { get; set; }
        public DbSet<OperationTypeDataModel> OperationType { get; set; }
        public DbSet<OpTypeRequirementsDataModel> OperationRequirements { get; set; }
        public DbSet<SurgeryRoomDataModel> SurgeryRoom { get; set; }
        public DbSet<SurgeryPhaseDataModel> SurgeryPhaseDataModel { get; set; }
        public DbSet<MaintenanceSlot> MaintenanceSlot { get; set; }
        public DbSet<AssignedEquipment> AssignedEquipment { get; set; }
        public DbSet<NeededPersonnelDataModel> AllocatedStaff { get; set; }
        public DbSet<SpecializationsDataModel> Specializations { get; set; }


        public BackOfficeDbContext(DbContextOptions<BackOfficeDbContext> options) : base(options) 
        { 
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
            .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information) // Enable SQL logging
            .UseMySql(
                "Server=vsgate-s1.dei.isep.ipp.pt;Port=11361;Database=hospitaldb;User=root;Password=K/C0QVM+rsI+;", 
                new MySqlServerVersion(new Version(8, 0, 5)),
                mysqlOptions => mysqlOptions.SchemaBehavior(MySqlSchemaBehavior.Ignore)
                
            );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<Name>();
            modelBuilder.Ignore<PhoneNumber>();
            // Apply the UserIdConverter for the UserId property
            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .HasConversion(new UserIdConverter()); 
            modelBuilder.Entity<LogDataModel>()
                .HasKey(l => l.LogId);

            modelBuilder.Entity<SurgeryRoomDataModel>()
                .HasKey(sr => sr.RoomNumber);

            modelBuilder.Entity<MaintenanceSlot>()
                .HasOne(ms => ms.SurgeryRoom)
                .WithMany(sr => sr.MaintenanceSlots)
                .HasForeignKey(ms => ms.RoomNumber)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AssignedEquipment>()
                .HasOne(ae => ae.SurgeryRoom)
                .WithMany(sr => sr.Equipments)
                .HasForeignKey(ae => ae.RoomNumber)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<AppointementDataModel>()
                .HasKey(np => np.AppointementId);  
            modelBuilder.Entity<NeededPersonnelDataModel>()
                .HasKey(np => new { np.StaffId, np.AppointementId });          

            modelBuilder.Entity<AvailableSlotDataModel>()
                .HasOne(s => s.Staff)
                .WithMany(s => s.AvailableSlots)
                .HasForeignKey(s => s.StaffId); 

            modelBuilder.Entity<StaffDataModel>()
                .HasKey(s => s.StaffId);     
                
            modelBuilder.Entity<OperationTypeDataModel>()
                .HasMany(o => o.Specializations)
                .WithOne(s => s.OperationType)
                .HasForeignKey(s => s.OperationTypeId)
                .OnDelete(DeleteBehavior.Cascade);   
                 
            modelBuilder.ApplyConfiguration(new UsersEntityTypeConfiguration());
            modelBuilder.Entity<SurgeryPhaseDataModel>()
                .HasOne(sp => sp.Appointement) // Navigation property to Appointement
                .WithMany(a => a.SurgeryPhases) // One-to-Many relationship
                .HasForeignKey(sp => sp.AppointementId) // Foreign key
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete

            // Configure SurgeryPhaseDataModel -> SurgeryRoomDataModel relationship
            modelBuilder.Entity<SurgeryPhaseDataModel>()
                .HasOne(sp => sp.SurgeryRoom) // Navigation property to SurgeryRoom
                .WithMany(sr => sr.Phases) // One-to-Many relationship
                .HasForeignKey(sp => sp.RoomNumber) // Foreign key
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete
                }

        public class UserIdConverter : ValueConverter<UserId, string>
        {
            public UserIdConverter() : base(
                id => id.AsString(), 
                str => new UserId(str) 
            )
            {
            }
        }
    }
}
