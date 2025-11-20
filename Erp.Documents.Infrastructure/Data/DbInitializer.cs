using Erp.Documents.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Erp.Documents.Infrastructure.Data
{
    /// <summary>
    /// Inicializador de base de datos: ejecuta migraciones y siembra datos iniciales.
    /// </summary>
    public static class DbInitializer
    {
        public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ErpDocumentsDbContext>();

                try
                {
                    // Aplicar migraciones pendientes
                    await context.Database.MigrateAsync();

                    // Sembrar datos iniciales
                    await SeedDataAsync(context);
                }
                catch (Exception ex)
                {
                    // Log en caso de error (en producción, usar ILogger)
                    Console.WriteLine($"Error al inicializar la base de datos: {ex.Message}");
                    throw;
                }
            }
        }

        private static async Task SeedDataAsync(ErpDocumentsDbContext context)
        {
            // No hacer nada si ya existen compañías
            if (context.Companies.Any())
            {
                return;
            }

            // Crear compañía de prueba
            var company = new Company
            {
                Id = Guid.NewGuid(),
                Name = "FailFast ERP - Demo Company",
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            context.Companies.Add(company);
            await context.SaveChangesAsync();
        }
    }
}
