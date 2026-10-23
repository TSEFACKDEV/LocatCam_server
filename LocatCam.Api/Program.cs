using AutoMapper;
using LocatCam.Api.Middleware;
using LocatCam.Infrastructure.Data;
using LocatCam.Infrastructure.Seed;
using LocatCam.Core.Interfaces;
using LocatCam.Core.Services;
using LocatCam.Core.Dtos;
using LocatCam.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using FluentValidation;
using System.Text;
using LocatCam.Core.Validation;

// --- Serilog : console + fichier ---
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/locatcam-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
    .MinimumLevel.Information()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Serilog comme provider de logs
builder.Host.UseSerilog();

// --- Configuration depuis appsettings (CreateBuilder les charge déjà,
//   suivi des variables d'environnement qui prennent le pas sur le JSON) ---
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// --- DbContext PostgreSQL ---
builder.Services.AddDbContext<LocatCamDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection") ??
        "Host=localhost;Database=locatcam;Username=postgres;Password=postgres;"));

// --- AutoMapper ---
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(LocatCam.Core.Services.MappingProfile).Assembly));

// --- FluentValidation : enregistrement manuel des validateurs -
// ASP.NET Core du00e9couvre automatiquement les IValidatoru003cTu003e pour valider les DTOs de contru00f4leur
builder.Services.AddTransient<IValidator<AnnonceCreateDto>, AnnonceCreateDtoValidator>();
builder.Services.AddTransient<IValidator<SignalementCreateDto>, SignalementCreateDtoValidator>();
builder.Services.AddTransient<IValidator<RegisterRequestDto>, RegisterRequestDtoValidator>();
builder.Services.AddTransient<IValidator<LoginRequestDto>, LoginRequestDtoValidator>();
builder.Services.AddTransient<IValidator<MessageCreateDto>, MessageCreateDtoValidator>();
builder.Services.AddTransient<IValidator<AnnonceUpdateDto>, AnnonceUpdateDtoValidator>();

// --- Répositories & Services métier ---
builder.Services.AddScoped(typeof(LocatCam.Core.Interfaces.IRepository<>), typeof(LocatCam.Infrastructure.Repositories.Repository<>));
builder.Services.AddScoped<LocatCam.Core.Interfaces.IUnitOfWork, LocatCam.Infrastructure.Repositories.UnitOfWork>();
builder.Services.AddScoped<LocatCam.Core.Interfaces.IAuthService, LocatCam.Infrastructure.Services.AuthService>();
builder.Services.AddScoped<LocatCam.Core.Interfaces.IAnnonceService, LocatCam.Infrastructure.Services.AnnonceService>();
builder.Services.AddScoped<LocatCam.Core.Interfaces.IPhotoService, LocatCam.Infrastructure.Services.PhotoService>();
builder.Services.AddScoped<LocatCam.Core.Interfaces.IFavoriService, LocatCam.Infrastructure.Services.FavoriService>();
builder.Services.AddScoped<LocatCam.Core.Interfaces.IMessageService, LocatCam.Infrastructure.Services.MessageService>();
builder.Services.AddScoped<LocatCam.Core.Interfaces.ISignalementService, LocatCam.Infrastructure.Services.SignalementService>();
builder.Services.AddScoped<LocatCam.Core.Interfaces.IAdminService, LocatCam.Infrastructure.Services.AdminService>();
builder.Services.AddScoped<ITokenService, LocatCam.Infrastructure.Services.TokenService>();

// --- Seed des données de test ---
builder.Services.AddScoped<LocatCam.Infrastructure.Seed.DataSeeder>();

// --- JWT Authentication ---
var jwtSection = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSection["SecretKey"] ?? "LocatCamSuperSecretKeyMin32Chars!!!";
var issuer = jwtSection["Issuer"] ?? "locatcam.api";
var audience = jwtSection["Audience"] ?? "locatcam.mobile";
var keyBytes = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// --- CORS pour le mobile ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("MobilePolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// --- Rate limiting sur /auth/* ---
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("AuthPolicy", config =>
    {
        config.PermitLimit = 10;
        config.Window = TimeSpan.FromMinutes(1);
        config.QueueLimit = 0;
    });
});

// --- OpenAPI / Swagger ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddControllers();

var app = builder.Build();

// --- Appliquer migrations + seed au démarrage ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    services.ApplyMigrationsAndSeed();
}

// --- Pipeline HTTP ---
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
}

app.UseSerilogRequestLogging();
app.UseCustomExceptionHandler();
app.UseCors("MobilePolicy");
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
