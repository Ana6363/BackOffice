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
using BackOffice.Domain.OperationType;
using BackOffice.Infraestructure.OperationTypes;
using BackOffice.Application.OperationTypes;
using Healthcare.Domain.Services;
using BackOffice.Application.Appointement;
using BackOffice.Domain.Specialization;
using BackOffice.Infraestructure.Specialization;
using BackOffice.Application.Specialization;
using BackOffice.Application.RoomType;
using BackOffice.Domain.RoomTypes;
using BackOffice.Infraestructure.RoomTypes;
using Azure.Messaging.ServiceBus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Add CORS policy to allow any origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin", builder =>
    {
        builder.AllowAnyOrigin() // Allow any origin
               .AllowAnyMethod() // Allow any HTTP method
               .AllowAnyHeader(); // Allow any header
    });
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle  
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Uncomment this line to enable Swagger
builder.Logging.AddConsole();

builder.Services.AddSingleton(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var connectionString = configuration["ServiceBus:ConnectionString"];

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException("Service Bus connection string is not configured.");
    }

    return new ServiceBusClient(connectionString);
});

// Get the connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("BackOfficeDb");

// Configure the DbContext to use MySQL
builder.Services.AddDbContext<BackOfficeDbContext>(options =>
    options
    .UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 5))) // Update with your MySQL version
    .EnableSensitiveDataLogging()
);

// Register services
builder.Services.AddHttpContextAccessor();
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
builder.Services.AddScoped<IOperationTypeRepository, OperationTypeRepository>();
builder.Services.AddScoped<OperationTypeService>();
builder.Services.AddScoped<SurgeryRoomService>();
builder.Services.AddHostedService<SurgeryRoomService>();
builder.Services.AddScoped<SurgeryRoomServiceProvider>();
builder.Services.AddScoped<AppointementService>();
builder.Services.AddScoped<ISpecializationRepository,SpecializationRepository>();
builder.Services.AddScoped<SpecializationService>();
builder.Services.AddScoped<RoomTypeService>();
builder.Services.AddScoped<IRoomTypeRepository,RoomTypeRepository>();
builder.Services.AddHttpClient<AllergyService>();
builder.Services.AddHttpClient<MedicalConditionsService>();
builder.Services.AddHttpClient<PatientMedicalRecordService>();



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

// Use CORS
app.UseCors("AllowAnyOrigin"); // Apply the CORS policy

// Configure the HTTP request pipeline.  
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run("http://0.0.0.0:5184");
