using Microsoft.EntityFrameworkCore; 
using Pomelo.EntityFrameworkCore.MySql.Infrastructure; 
using BackOffice.Domain.Users; 
using BackOffice.Infrastructure.Users; 
using Microsoft.EntityFrameworkCore.Storage.ValueConversion; 

namespace BackOffice.Infrastructure
{
    public class BackOfficeDbContext : DbContext
    {
        public DbSet<User> Users { get; set; } 

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
            // Apply the UserIdConverter for the UserId property
            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .HasConversion(new UserIdConverter());
            
            modelBuilder.ApplyConfiguration(new UsersEntityTypeConfiguration());
        }

        // Value Converter for UserId
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
