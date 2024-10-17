using BackOffice.Infrastructure;
using BackOffice.Domain.Users;
using BackOffice.Infrastructure.Users; 
using Microsoft.EntityFrameworkCore;
using BackOffice.Domain.Shared;
using BackOffice.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using BackOffice.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.  
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle  
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Uncomment this line to enable Swagger

// Get the connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("BackOfficeDb");

// Configure the DbContext to use MySQL
builder.Services.AddDbContext<BackOfficeDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 5))) // Update with your MySQL version
);

// Register services
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IUserRepository, UsersRepository>();
builder.Services.AddTransient<UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();


var app = builder.Build();

// Configure the HTTP request pipeline.  
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
