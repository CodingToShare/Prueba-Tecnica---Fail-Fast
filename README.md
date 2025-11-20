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

Esta es la **Meta 5** completada. Las metas completadas incluyen:
- ✅ **Meta 1**: Estructura de proyectos y DI
- ✅ **Meta 2**: Modelo de datos (Migraciones EF Core)
- ✅ **Meta 3**: Servicios de almacenamiento (Azure Blob, S3)
- ✅ **Meta 4**: Casos de uso y lógica de validación
- ✅ **Meta 5**: Endpoints REST
- ⏳ **Meta 6**: Validación, auditoría, manejo de errores
- ⏳ **Meta 7**: Tests unitarios e integration tests
- ⏳ **Meta 8**: Docker y documentación final

## 📝 Paquetes NuGet Instalados

- `Swashbuckle.AspNetCore` - Swagger/OpenAPI
- `Microsoft.EntityFrameworkCore.SqlServer` - Data Access
- `Microsoft.EntityFrameworkCore.Tools` - Migrations
- `Azure.Storage.Blobs` - Azure Blob Storage
- `AWSSDK.S3` - AWS S3
- `FluentValidation` - Validación de DTOs
- `Microsoft.Extensions.Logging.Abstractions` - Logging
- `Microsoft.Extensions.Options` - Options pattern

## 📦 Meta 4: Servicios de Aplicación

### Servicios Implementados

1. **UploadDocumentService**
   - `InitiateUploadAsync`: Crea documento, flujo de validación (si aplica), genera URL presignada
   - `CompleteUploadAsync`: Verifica archivo en storage, genera URL de descarga
   - Validación de tamaño máximo, MIME types, campos requeridos

2. **DownloadDocumentService**
   - `GetDownloadUrlAsync`: Genera URL presignada con metadatos (nombre, tipo, tamaño, fecha expiracion)
   - Verifica existencia del archivo en storage

3. **ApproveDocumentService**
   - `ApproveAsync`: Aprueba paso actual, avanza al siguiente (o marca completamente aprobado)
   - `GetValidationStatusAsync`: Retorna estado completo del flujo de validación
   - Lógica jerárquica multi-paso con auditoría completa

4. **RejectDocumentService**
   - `RejectAsync`: Marca documento como rechazado (estado terminal R)
   - Marca todos los pasos pendientes como rechazados
   - Registra acción de rechazo con motivo y auditoría

### DTOs Creados

**Request/Response:**
- `UploadDocumentRequest` → `UploadDocumentResponse`
- `DownloadDocumentResponse`
- `ApproveDocumentRequest`
- `RejectDocumentRequest`
- `DocumentOperationResponse`

**Status:**
- `ValidationFlowStatusDto`: Estado completo del flujo
- `ValidationStepStatusDto`: Estado individual de cada paso

### Integración DI (Program.cs)

```csharp
builder.Services.AddScoped<IUploadDocumentService, UploadDocumentService>();
builder.Services.AddScoped<IDownloadDocumentService, DownloadDocumentService>();
builder.Services.AddScoped<IApproveDocumentService, ApproveDocumentService>();
builder.Services.AddScoped<IRejectDocumentService, RejectDocumentService>();
```

## 📡 Meta 5: REST Controllers

### Endpoints Implementados

#### 1. Upload Controller (`/api/upload`)

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/initiate` | Inicia carga, genera URL presignada |
| POST | `/{documentId}/complete` | Completa carga, verifica en storage |

**Request (Initiate):**
```json
{
  "companyId": "guid",
  "entityType": "Invoice",
  "entityId": "INV-001",
  "fileName": "invoice.pdf",
  "mimeType": "application/pdf",
  "fileSizeBytes": 102400,
  "requiresValidation": true,
  "uploadedByUserId": "user-123"
}
```

**Response:**
```json
{
  "documentId": "guid",
  "presignedUploadUrl": "https://...",
  "bucketKey": "documents/company-.../...",
  "expiresInMinutes": 15,
  "status": "Pending"
}
```

#### 2. Download Controller (`/api/download`)

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/{documentId}` | Genera URL presignada de descarga |

**Response:**
```json
{
  "documentId": "guid",
  "presignedDownloadUrl": "https://...",
  "fileName": "invoice.pdf",
  "mimeType": "application/pdf",
  "fileSizeBytes": 102400,
  "expiresInMinutes": 15,
  "status": "Available"
}
```

#### 3. Validation Controller (`/api/validation`)

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/approve` | Aprueba documento (avanza paso o completa) |
| POST | `/reject` | Rechaza documento (estado terminal) |
| GET | `/{documentId}/status` | Obtiene estado del flujo de validación |

**Request (Approve):**
```json
{
  "documentId": "guid",
  "approverUserId": "user-456",
  "reason": "Documento revisado correctamente"
}
```

**Request (Reject):**
```json
{
  "documentId": "guid",
  "rejecterUserId": "user-456",
  "reason": "Falta información crítica"
}
```

**Response (Status):**
```json
{
  "documentId": "guid",
  "currentStatus": "P",
  "totalSteps": 3,
  "completedSteps": 1,
  "steps": [
    {
      "order": 1,
      "status": "Approved",
      "approverUserId": "guid",
      "approvedAtUtc": "2025-11-20T10:30:00Z",
      "reason": "Revisado"
    }
  ]
}
```

### HTTP Status Codes

| Código | Significado |
|--------|------------|
| 200 | Operación exitosa |
| 400 | Solicitud inválida (validación, estado, etc.) |
| 404 | Recurso no encontrado |
| 500 | Error interno del servidor |

### Error Response

```json
{
  "error": "Descripción del error"
}
```

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
✅ **Meta 3: Servicios de almacenamiento multi-cloud (Azure Blob, S3)**  
✅ **Meta 4: Servicios de aplicación (Upload, Download, Approve, Reject)**  
✅ **Meta 5: REST Controllers (Upload, Download, Validation)**  

⏳ Próximo: Validación, auditoría, manejo de errores (Meta 6)
