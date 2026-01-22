using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CagnotteSolidaire.Infrastructure.Data;
using CagnotteSolidaire.Domain.Entities;
using CagnotteSolidaire.Domain.Interfaces;
using CagnotteSolidaire.Infrastructure.Repositories;
using CagnotteSolidaire.Infrastructure.ExternalServices;
using CagnotteSolidaire.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);



// ========== Configuration de la Base de Données ==========
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("CagnotteSolidaire.Infrastructure")));


// ========== Configuration d'Identity ==========
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Configuration des mots de passe
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    // Configuration de l'email
    options.User.RequireUniqueEmail = true;

    // Configuration du lockout
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ========== Configuration JWT ==========
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

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
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ========== MediatR (CQRS) ==========
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(CagnotteSolidaire.Application.Common.Interfaces.ICommand<>).Assembly));

// ========== Repositories & Unit of Work ==========
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAssociationRepository, AssociationRepository>();
builder.Services.AddScoped<ICagnotteRepository, CagnotteRepository>();
builder.Services.AddScoped<IParticipationRepository, ParticipationRepository>();

// ========== Services ==========
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

// ========== Services Externes ==========
builder.Services.AddHttpClient<IJOAssociationsService, JOAssociationsService>();


// ========== Controllers & Swagger ==========
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ========== CORS (pour Blazor) ==========

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.WithOrigins("https://localhost:5001", "http://localhost:5000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// ========== Initialisation des rôles ==========
await InitializeRolesAsync(app);

// ========== Middleware Pipeline ==========
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazor");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

// Méthode pour créer les rôles
static async Task InitializeRolesAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    
    string[] roles = { "Participant", "Gestionnaire" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}
