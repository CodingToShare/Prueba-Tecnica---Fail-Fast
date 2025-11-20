using Microsoft.EntityFrameworkCore;
using Erp.Documents.Infrastructure.Data;
using Erp.Documents.Infrastructure.Configuration;
using Erp.Documents.Application.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ===== Configuración de opciones =====
builder.Services.Configure<StorageOptions>(
    builder.Configuration.GetSection(StorageOptions.SectionName));
builder.Services.Configure<ValidationOptions>(
    builder.Configuration.GetSection(ValidationOptions.SectionName));
builder.Services.Configure<SecurityOptions>(
    builder.Configuration.GetSection(SecurityOptions.SectionName));
builder.Services.Configure<ApiOptions>(
    builder.Configuration.GetSection(ApiOptions.SectionName));
builder.Services.Configure<AuditOptions>(
    builder.Configuration.GetSection(AuditOptions.SectionName));

// ===== Servicios de base de datos =====
builder.Services.AddDbContext<ErpDocumentsDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.MigrationsAssembly("Erp.Documents.Infrastructure")
    )
);

// ===== Repositorios =====
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IValidationFlowRepository, ValidationFlowRepository>();

// ===== Servicios de aplicación (placeholder - se rellenarán con las metas) =====
// Se agregarán aquí los servicios de aplicación, etc.

// ===== Swagger/OpenAPI =====
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

var app = builder.Build();

// ===== Inicializar Base de Datos =====
await DbInitializer.InitializeDatabaseAsync(app.Services);

// ===== Middleware =====
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Erp.Documents API v1");
        c.RoutePrefix = string.Empty; // Swagger en raíz
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

