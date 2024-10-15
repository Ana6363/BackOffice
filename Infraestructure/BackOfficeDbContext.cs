using Microsoft.EntityFrameworkCore;
using BackOffice.Domain.Users;
using BackOffice.Infrastructure.Users;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace BackOffice.Infrastructure
{
    public class BackOfficeDbContext : DbContext
    {
        public DbSet<User> Users { get; set; } 

        public BackOfficeDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UsersEntityTypeConfiguration()); 
        }
    }
}