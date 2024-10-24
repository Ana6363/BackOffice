using BackOffice.Infrastructure;
using BackOffice.Domain.Users;
using BackOffice.Infrastructure.Users; 
using Microsoft.EntityFrameworkCore;
using BackOffice.Domain.Shared;
using BackOffice.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using BackOffice.Infrastructure.Services;
using BackOffice.Application.Services;
using BackOffice.Application.Users;
using BackOffice.Application.OAuth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BackOffice.Domain.Patients;
using BackOffice.Infrastructure.Persistence.Repositories;
using BackOffice.Application.Patients;
using BackOffice.Infrastructure.Staff;
using BackOffice.Application.StaffService;
using BackOffice.Domain.OperationRequest;
using BackOffice.Infraestructure.OperationRequest;
using BackOffice.Application.OperationRequest;
using BackOffice.Domain.Appointement;
using BackOffice.Infraestructure.Appointement;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle  
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Uncomment this line to enable Swagger
builder.Logging.AddConsole();

// Get the connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("BackOfficeDb");

// Configure the DbContext to use MySQL
builder.Services.AddDbContext<BackOfficeDbContext>(options =>
    options
    .UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 5))) // Update with your MySQL version
    .EnableSensitiveDataLogging()
);

// Register services
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IUserRepository, UsersRepository>();
builder.Services.AddTransient<UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<UserActivationService>();
builder.Services.AddScoped<IGoogleOAuthService, GoogleOAuthService>();
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<PatientService>();
builder.Services.AddScoped<IStaffRepository,StaffRepository>();
builder.Services.AddScoped<StaffService>();
builder.Services.AddScoped<IOperationRequestRepository,OperationRequestRepository>();
builder.Services.AddScoped<OperationRequestService>();
builder.Services.AddScoped<IAppointementRepository, AppointementRepository>();

    // Configure Authentication
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"], // Pulling from appsettings.json
            ValidAudience = builder.Configuration["Jwt:Audience"], // Pulling from appsettings.json
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) // Pulling from appsettings.json
        };
    });

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
