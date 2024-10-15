using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BackOffice.Domain.Users;

namespace BackOffice.Infrastructure.Users
{
    internal class UsersEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users", SchemaNames.BackOffice);
            builder.HasKey(b => b.Id); 

            //builder.Property<bool>(b => b.Active).HasColumnName("Active");
        }
    }
}