using System.Text;
using ImprentaSR.Application.Interfaces;
using ImprentaSR.Application.UseCases.Clientes;
using ImprentaSR.Application.UseCases.Pedidos;
using ImprentaSR.Application.UseCases.Productos;
using ImprentaSR.Application.Validators;
using ImprentaSR.Domain.Entities;
using ImprentaSR.Domain.Interfaces;
using ImprentaSR.Infrastructure.Data;
using ImprentaSR.Infrastructure.Repositories;
using ImprentaSR.WebAPI.Middleware;
using ImprentaSR.WebAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────────
builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<OtpService>();

// Database - Dapper + SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=localhost;Database=ImprentaDB;Trusted_Connection=True;TrustServerCertificate=True;";

builder.Services.AddSingleton(new DbConnectionFactory(connectionString));
builder.Services.AddScoped<DatabaseInitializer>();

// Dependency Injection - Clean Architecture
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<ISriValidator, SriValidator>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddHttpClient<SriService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(15);
    client.BaseAddress = new Uri("https://srienlinea.sri.gob.ec/sri-catastro-sujeto-servicio-internet/rest/");
    client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; ImprentaSR/1.0)");
});

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
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
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
    };
});

builder.Services.AddAuthorization();

// CORS - allow frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ── Database Initialization ───────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await initializer.InitializeAsync();
}

// ── Middleware Pipeline ───────────────────────────────────────
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
