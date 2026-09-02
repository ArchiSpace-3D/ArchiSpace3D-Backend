using ArchiSpace3D.Api.Dao;
using ArchiSpace3D.Api.Data;
using ArchiSpace3D.Api.Hubs;
using ArchiSpace3D.Api.Service;
using ArchiSpace3D.Api.Util;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Registro del DbContext con PostgreSQL (Database First / scaffold)
builder.Services.AddDbContext<ArchiSpaceContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Swagger / OpenAPI -- con soporte para mandar el token Bearer desde la UI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pega solo el token (sin la palabra 'Bearer')"
    });

    // AddSecurityRequirement ahora recibe un delegate con el "document"
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

// SignalR: incluido en ASP.NET Core, no requiere paquete NuGet adicional
builder.Services.AddSignalR();

// CORS: SignalR con WebSockets necesita AllowCredentials(), por eso NO se
// puede usar AllowAnyOrigin() a la vez.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// ===== Autenticación JWT =====
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSettings["Key"]!;

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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };

    // Necesario para que SignalR pueda autenticar la conexión del Hub:
    // SignalR con WebSockets no puede mandar el token en el header
    // Authorization normal, lo manda como query string (?access_token=...),
    // así que aquí se le dice a JwtBearer que también lo busque ahí.
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<usuarioDAOImpl, usuarioDao>();
builder.Services.AddScoped<proyectoDAOImpl, proyectoDAO>();
builder.Services.AddScoped<elementoEstructuralDAOImpl, elementoEstructuralDAO>();
builder.Services.AddScoped<espacioFisicoDAOImpl, espacioFisicoDAO>();
builder.Services.AddScoped<invitacionDAOImpl, invitacionDAO>();
builder.Services.AddScoped<medicionDAOImpl, medicionDAO>();
builder.Services.AddScoped<modeloimportadoDAOImpl, modeloImportadoDAO>();
builder.Services.AddScoped<notificacionDAOImpl, notificacionDAO>();
builder.Services.AddScoped<versiondiseñoDAOImpl, versiondisenoDAO>();

// Services
builder.Services.AddScoped<usuarioServiceImpl, usuarioService>();
builder.Services.AddScoped<proyectoServiceImpl, proyectoService>();
builder.Services.AddScoped<elementoEstructuralServiceImpl, elementoEstructuralService>();
builder.Services.AddScoped<espacioFisicoServiceImpl, espacioFisicoService>();
builder.Services.AddScoped<invitacionServiceImpl, invitacionService>();
builder.Services.AddScoped<medicionServiceImpl, medicionService>();
builder.Services.AddScoped<modeloImportadoServiceImpl, modeloImportadoService>();
builder.Services.AddScoped<notificacionServiceImpl, notificacionService>();
builder.Services.AddScoped<versionDiseñoServiceImpl, versionDiseñoService>();

// Autenticación
builder.Services.AddSingleton<JwtTokenGenerator>();
builder.Services.AddScoped<AuthServiceImpl, AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

// El orden importa: Authentication SIEMPRE antes que Authorization.
// Authentication responde "¿quién eres?" (lee y valida el token).
// Authorization responde "¿tienes permiso?" ([Authorize], [Authorize(Roles=...)]).
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<SalaColaborativaHub>("/hubs/sala");

app.Run();