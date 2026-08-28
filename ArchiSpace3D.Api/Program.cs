using ArchiSpace3D.Api.Controllers;
using ArchiSpace3D.Api.Dao;
using ArchiSpace3D.Api.Data;
using ArchiSpace3D.Api.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Registro del DbContext con PostgreSQL (Database First / scaffold)
builder.Services.AddDbContext<ArchiSpaceContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SchemaFilter<IgnoreNavigationSchemaFilter>();
});

// DAOs
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
builder.Services.AddScoped<versionDiseñoServiceImpl, versionDisenoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Filtro para ocultar propiedades de navegación en el schema de Swagger
public class IgnoreNavigationSchemaFilter : ISchemaFilter
{
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema is OpenApiSchema openApiSchema && openApiSchema.Properties is not null)
        {
            var propsToRemove = openApiSchema.Properties
                .Where(p => p.Key.Contains("Navigation"))
                .Select(p => p.Key).ToList();

            foreach (var prop in propsToRemove)
                openApiSchema.Properties.Remove(prop);
        }
    }
}