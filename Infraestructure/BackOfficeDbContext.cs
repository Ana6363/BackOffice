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

            modelBuilder.Entity<AvailableSlotDataModel>()
                .HasOne(s => s.Staff)
                .WithMany(s => s.AvailableSlots)
                .HasForeignKey(s => s.StaffId); 

            modelBuilder.Entity<StaffDataModel>()
                .HasKey(s => s.StaffId);     
                
            modelBuilder.ApplyConfiguration(new UsersEntityTypeConfiguration());
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
