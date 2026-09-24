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
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

var builder = WebApplication.CreateBuilder(args);

// Firebase Admin SDK: en producción (Railway) las credenciales vienen de la
// variable de entorno FIREBASE_CREDENTIALS_JSON (JSON completo o Base64), así
// la key no vive en el repo ni en la imagen. En local, si la variable no
// existe, se usa el archivo de Config/ como antes.
var firebaseRaw = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS_JSON");
GoogleCredential? firebaseCredential = null;

if (!string.IsNullOrWhiteSpace(firebaseRaw))
{
    var firebaseJson = firebaseRaw.Trim();

    if (!firebaseJson.StartsWith("{"))
    {
        try
        {
            firebaseJson = Encoding.UTF8.GetString(Convert.FromBase64String(firebaseJson));
        }
        catch (FormatException)
        {
            Console.WriteLine("Advertencia: FIREBASE_CREDENTIALS_JSON no es JSON ni Base64 valido.");
        }
    }

    if (firebaseJson.StartsWith("{"))
    {
        firebaseCredential = GoogleCredential.FromJson(firebaseJson);
    }
}
else
{
    var credentialPath = Path.Combine(builder.Environment.ContentRootPath, "Config", "archispace3d-firebase-adminsdk-fbsvc-4d0bd37b46.json");

    if (!File.Exists(credentialPath))
    {
        Console.WriteLine("Advertencia: No se encontraron credenciales de Firebase en " + credentialPath + ". Push desactivado.");
    }
    else
    {
        firebaseCredential = GoogleCredential.FromFile(credentialPath);
    }
}

if (firebaseCredential != null)
{
    FirebaseApp.Create(new AppOptions()
    {
        Credential = firebaseCredential
    });
}


builder.Services.AddControllers();

builder.Services.AddDbContext<ArchiSpaceContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

builder.Services.AddSignalR();


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

builder.Services.AddHttpClient<usuarioServiceImpl, usuarioService>();
builder.Services.AddScoped<proyectoServiceImpl, proyectoService>();
builder.Services.AddScoped<elementoEstructuralServiceImpl, elementoEstructuralService>();
builder.Services.AddScoped<espacioFisicoServiceImpl, espacioFisicoService>();
builder.Services.AddScoped<invitacionServiceImpl, invitacionService>();
builder.Services.AddScoped<medicionServiceImpl, medicionService>();
builder.Services.AddScoped<modeloImportadoServiceImpl, modeloImportadoService>();
builder.Services.AddScoped<notificacionServiceImpl, notificacionService>();
builder.Services.AddScoped<versionDiseñoServiceImpl, versionDiseñoService>();
builder.Services.AddScoped<pushNotificationServiceImpl, pushNotificationService>();

builder.Services.AddSingleton<JwtTokenGenerator>();
builder.Services.AddScoped<AuthServiceImpl, AuthService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("AllowAll");



app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<SalaColaborativaHub>("/hubs/sala");

app.Run();

