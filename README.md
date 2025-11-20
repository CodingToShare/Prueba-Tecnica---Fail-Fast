# Erp.Documents - Gestión de Documentos para ERP

## 📋 Descripción

Solución de microservicio backend para gestión de documentos en un ERP con soporte para:
- **Almacenamiento multi-cloud**: Azure Blob Storage y AWS S3
- **Validación jerárquica de documentos**: Flujo de aprobación con múltiples niveles
- **API REST JSON**: Endpoints para carga, descarga, aprobación y rechazo de documentos
- **Arquitectura limpia**: Separación clara de responsabilidades (Domain, Application, Infrastructure, Api)

## 🏗️ Estructura del Proyecto

```
Erp.Documents/
├── Erp.Documents.Api/           # Web API (Controllers, Middleware, Filters)
├── Erp.Documents.Application/   # Casos de uso (Services, DTOs, Validators)
├── Erp.Documents.Domain/        # Entidades de dominio (Entities, Enums, Interfaces)
├── Erp.Documents.Infrastructure/ # Data Access, Storage, Configuration
└── Erp.Documents.sln
```

## ⚙️ Configuración

### Environment: Development

**appsettings.Development.json** incluye:
- Connection string a SQL Server Azure
- Azure Blob Storage credentials (proveedor por defecto)
- AWS S3 config (disponible para cambiar)
- Opciones de validación, seguridad, auditoría

### Variables de Entorno (Opcional)

```bash
Storage:Provider=AzureBlob
Storage:Azure:ConnectionString=...
Storage:Azure:ContainerName=documents
```

## 🚀 Instrucciones de Ejecución (Próximamente)

Esta es la **Meta 1** completada. Las próximas metas incluirán:
- **Meta 2**: Modelo de datos completo (Migraciones EF Core)
- **Meta 3**: Servicios de almacenamiento (Azure Blob, S3)
- **Meta 4**: Casos de uso y lógica de validación
- **Meta 5**: Endpoints REST
- **Meta 6**: Validación, auditoría, manejo de errores
- **Meta 7**: Tests unitarios e integration tests
- **Meta 8**: Docker y documentación final

## 📝 Paquetes NuGet Instalados

- `Swashbuckle.AspNetCore` - Swagger/OpenAPI
- `Microsoft.EntityFrameworkCore.SqlServer` - Data Access
- `Microsoft.EntityFrameworkCore.Tools` - Migrations
- `Azure.Storage.Blobs` - Azure Blob Storage
- `AWSSDK.S3` - AWS S3
- `FluentValidation` - Validación de DTOs

## 🔧 Estado Actual

✅ Estructura de proyectos creada  
✅ Configuración de DI en Program.cs  
✅ Entidades de dominio definidas  
✅ DbContext configurado  
✅ Opciones de configuración estructuradas  
✅ appsettings.Development.json con credenciales reales  
✅ **Migraciones EF Core creadas y automatizadas**  
✅ **Repositorios implementados (Document, ValidationFlow)**  
✅ **DbInitializer: Auto-migración y seed de datos**  

⏳ Próximo: Implementar servicios de almacenamiento multi-cloud (Meta 3)
