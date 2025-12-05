using PetSafe.Config;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using PetSafe.Infraestructure.Repositories;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PetSafe.Application.Interfaces;
using PetSafe.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------
// 1. Controllers + JSON Serialization (camelCase)
// ------------------------------------------------------------
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// ------------------------------------------------------------
// 1.1. CORS
// ------------------------------------------------------------
builder.Services.AddCors(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        // Política mais permissiva para desenvolvimento
        options.AddPolicy("AllowFrontend", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    }
    else
    {
        // Política restritiva para produção
        // Permite configurar origens via variável de ambiente
        var allowedOrigins = builder.Configuration["Cors:AllowedOrigins"]?.Split(',') 
            ?? new[] { "http://localhost:3000", "http://localhost:5173" };
        
        options.AddPolicy("AllowFrontend", policy =>
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
    }
});

// ------------------------------------------------------------
// 2. Swagger + JWT Authorize button
// ------------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Insira o token JWT desta forma: Bearer {seu_token}",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// ------------------------------------------------------------
// 3. Bind / Register JwtSettings (o que faltava!)
// ------------------------------------------------------------
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtSettings = jwtSection.Get<JwtSettings>();

var key = Encoding.ASCII.GetBytes(jwtSettings?.Key ?? "MysuperSecretKey12345");

// ------------------------------------------------------------
// 4. Authentication + JWT
// ------------------------------------------------------------
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),

            ValidateIssuer = true,
            ValidIssuer = jwtSettings?.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtSettings?.Audience,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ------------------------------------------------------------
// 5. Repositories e Services
// ------------------------------------------------------------
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, AuthService>();
builder.Services.AddScoped<IPetRepository, PetRepository>();
builder.Services.AddScoped<IPetService, PetService>();
builder.Services.AddScoped<IVaccineRepository, VaccineRepository>();
builder.Services.AddScoped<IVaccineService, VaccineService>();
builder.Services.AddScoped<IWeightRecordRepository, WeightRecordRepository>();
builder.Services.AddScoped<IWeightRecordService, WeightRecordService>();

// ------------------------------------------------------------
// 6. Database
// ------------------------------------------------------------
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(conn, new MySqlServerVersion(new Version(8, 0, 21))));

var app = builder.Build();

// ------------------------------------------------------------
// 7. Pipeline
// ------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS DEVE vir ANTES de UseHttpsRedirection e UseAuthentication
app.UseCors("AllowFrontend");

// Em desenvolvimento, podemos pular o redirecionamento HTTPS para evitar problemas com CORS
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
