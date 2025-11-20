using Microsoft.EntityFrameworkCore;
using Erp.Documents.Infrastructure.Data;
using Erp.Documents.Infrastructure.Configuration;
using Erp.Documents.Infrastructure.Storage;
using Erp.Documents.Application.Interfaces;
using Amazon.S3;

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

// ===== Servicios de Almacenamiento Multi-Cloud (Meta 3) =====
// Registrar ambas implementaciones (Azure y S3)
builder.Services.AddScoped<AzureBlobStorageService>();
builder.Services.AddScoped<S3StorageService>();

// Registrar cliente S3 usando constructor con credentials de appsettings
// Las credenciales deben venir de ambiente en producción (env vars o Secrets Manager)
builder.Services.AddScoped<IAmazonS3>(provider =>
{
    var storageOptions = builder.Configuration.GetSection(StorageOptions.SectionName).Get<StorageOptions>();
    if (storageOptions?.Provider == "AwsS3" && storageOptions.AwsS3 != null)
    {
        var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(storageOptions.AwsS3.Region);
        // Si usas credenciales de ambiente, puedes usar DefaultAWSCredentials
        return new AmazonS3Client(regionEndpoint);
    }
    // Retornar una instancia dummy si no es S3
    return null!;
});

// Registrar factory y resolver según configuración
builder.Services.AddScoped<IStorageServiceFactory, StorageServiceFactory>();
builder.Services.AddScoped<IObjectStorageService>(provider =>
{
    var factory = provider.GetRequiredService<IStorageServiceFactory>();
    return factory.CreateStorageService();
});

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

