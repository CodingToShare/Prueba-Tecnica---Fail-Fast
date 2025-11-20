# Gestión de Documentos en ERP

Módulo backend en **.NET 10** para almacenamiento de documentos de negocio con cloud storage (Azure Blob Storage o AWS S3) y validación jerárquica de aprobaciones.

## 📦 Contexto

Este módulo permite almacenar documentos de negocio (fotos de vehículos, documentos de empleados, certificados, etc.) en cloud storage, mantener sus metadatos en base de datos SQL Server y gestionar un flujo jerárquico de validación con aprobaciones.

## 🎯 Alcance Implementado

✅ **Modelado de datos** con ORM (Entity Framework Core):
- Empresas y entidades de dominio (genéricas: EntityType, EntityId)
- Documentos con metadatos (nombre, tipo MIME, tamaño, ubicación en bucket)
- Flujo de validación jerárquico con pasos y acciones

✅ **Endpoint de carga** (`POST /api/upload/initiate` y `/complete`):
- Genera URL presignada para subir al bucket
- Crea referencia en BD sin almacenar binario

✅ **Endpoint de descarga** (`GET /api/download/{documentId}`):
- Retorna URL presignada para descargar
- Incluye metadatos y estado de validación

✅ **Acciones de validación**:
- `POST /api/validation/approve` - Aprueba documento
- `POST /api/validation/reject` - Rechaza documento (terminal)
- Reglas de jerarquía: mayor order aprueba pasos previos

✅ **Auditoría y trazabilidad**: Registra todas las acciones

## 📋 Modelo de Datos

```
Company
  └─ Document (companyId, entityType, entityId, name, mimeType, sizeBytes, bucketKey)
      ├─ ValidationStatus: NULL | "P" | "A" | "R"
      └─ DocumentValidationFlow
          ├─ ValidationStep (order, approverUserId, status)
          └─ ValidationAction (actionType, actorUserId, reason)
```

**Estados de validación:**
- `NULL` - Sin validación requerida
- `P` - Pendiente
- `A` - Aprobado
- `R` - Rechazado (terminal)

## 🔧 Configuración

### Variables de Entorno

Crear archivo `.env` o configurar en `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ErpDocuments;User Id=sa;Password=YourPassword;TrustServerCertificate=true;"
  },
  "Storage": {
    "Provider": "AzureBlob"
  }
}
```

### Opción 1: Azure Blob Storage

1. **Crear Storage Account en Azure**:
   - Nombre: `mystorageaccount`
   - Crear contenedor: `documents`

2. **Configurar en appsettings.Development.json**:

```json
{
  "Storage": {
    "Provider": "AzureBlob",
    "Azure": {
      "ConnectionString": "DefaultEndpointsProtocol=https;AccountName=mystorageaccount;AccountKey=ACCOUNT_KEY;EndpointSuffix=core.windows.net",
      "ContainerName": "documents"
    }
  }
}
```

3. **O usar variables de entorno**:

```bash
export Storage__Provider=AzureBlob
export Storage__Azure__ConnectionString="DefaultEndpointsProtocol=https;AccountName=...;AccountKey=...;EndpointSuffix=core.windows.net"
export Storage__Azure__ContainerName=documents
```

### Opción 2: AWS S3

1. **Crear bucket en AWS S3**:
   - Nombre: `erp-documents`
   - Región: `us-east-1`

2. **Configurar en appsettings.Development.json**:

```json
{
  "Storage": {
    "Provider": "S3",
    "S3": {
      "BucketName": "erp-documents",
      "Region": "us-east-1"
    }
  }
}
```

3. **Configurar credenciales AWS** (una de estas opciones):

**Opción A: Variables de entorno**
```bash
export AWS_ACCESS_KEY_ID=YOUR_ACCESS_KEY
export AWS_SECRET_ACCESS_KEY=YOUR_SECRET_KEY
export AWS_REGION=us-east-1
```

**Opción B: Archivo ~/.aws/credentials** (Linux/Mac)
```
[default]
aws_access_key_id = YOUR_ACCESS_KEY
aws_secret_access_key = YOUR_SECRET_KEY
```

**Opción C: Archivo %USERPROFILE%\.aws\credentials** (Windows)
```
[default]
aws_access_key_id = YOUR_ACCESS_KEY
aws_secret_access_key = YOUR_SECRET_KEY
```

## 🗄️ Setup Inicial de Base de Datos

**IMPORTANTE:** Ejecuta esto ANTES de ejecutar la API.

### Opción Recomendada: Script PowerShell Automatizado

```powershell
# 1. Navegar a la carpeta scripts
cd "scripts"

# 2. Ejecutar setup (crea BD, tablas y datos de prueba)
.\setup-database.ps1

# 3. Copiar connection string de CONNECTION_STRING.txt
#    a appsettings.Development.json
```

**Qué hace:**
- ✓ Crea BD `ErpDocuments` en SQL Server
- ✓ Crea 5 tablas con índices
- ✓ Inserta datos de prueba (2 empresas, 3 documentos, 3 pasos de validación)
- ✓ Ejecuta EF Core migrations
- ✓ Genera `CONNECTION_STRING.txt`

**Detalles:** Ver [`scripts/README.md`](scripts/README.md)

### Opción Manual: SQL Script

```bash
sqlcmd -S localhost -U sa -P "YourPassword123!" -i scripts/setup-database.sql
```

---

## 🚀 Ejecución

### Opción 1: Desarrollo Local

**Requisitos previos:**
- .NET 10.0 SDK
- SQL Server 2022+ (local o Docker)
- ✅ Base de datos creada (ver sección anterior)

**Pasos:**

```bash
# 1. Clonar repositorio
git clone https://github.com/CodingToShare/Prueba-Tecnica---Fail-Fast.git
cd "Prueba Tecnica - Fail Fast"

# 2. Restaurar paquetes
dotnet restore

# 3. Configurar connection string
# Copiar de CONNECTION_STRING.txt (después de ejecutar setup-database.ps1)
# a appsettings.Development.json

# 4. Ejecutar API
cd Erp.Documents.Api
dotnet run --launch-profile https
```

**API disponible en:** `https://localhost:5001`

**Swagger UI:** `https://localhost:5001/swagger`

### Opción 2: Docker Compose

**Requisitos:**
- Docker Desktop

**Pasos:**

```bash
# Construir e iniciar servicios
docker-compose up -d

# Verificar servicios
docker ps

# Ver logs de API
docker logs erp-documents-api

# Ver logs de SQL Server
docker logs erp-documents-db
```

**API disponible en:** `http://localhost:8080`

**Swagger UI:** `http://localhost:8080/swagger`

**Detener servicios:**
```bash
docker-compose down
```

## 🧪 Pruebas

### Ejecutar Tests

```bash
cd Erp.Documents.Tests
dotnet test
```

**Resultado esperado:** 13 tests pasando
- 10 tests de validadores
- 4 tests de entidades
- 2 tests de servicios

## 💻 Estructura del Proyecto

```
Erp.Documents.Api/
  ├── Controllers/              # Endpoints REST
  ├── Middleware/               # Manejo global de excepciones
  └── Program.cs               # Configuración y DI

Erp.Documents.Application/
  ├── DTOs/                    # Request/Response models
  ├── Interfaces/              # Contratos de servicios
  └── Validators/              # FluentValidation rules

Erp.Documents.Domain/
  ├── Entities/                # Document, ValidationFlow, ValidationStep, etc.
  ├── Enums/                   # ValidationStatus, etc.
  └── Events/                  # Domain events

Erp.Documents.Infrastructure/
  ├── Data/                    # DbContext, Repositories, Migraciones
  ├── Services/                # Upload, Download, Approve, Reject, Storage
  └── Configuration/           # Options, DbContext config

Erp.Documents.Tests/
  ├── Unit/                    # Validator tests
  └── Integration/             # Entity tests
```

## 📡 API Endpoints

### Carga de Documento

**POST** `/api/upload/initiate`

Request:
```json
{
  "companyId": "550e8400-e29b-41d4-a716-446655440000",
  "entityType": "vehicle",
  "entityId": "VEH-001",
  "fileName": "soat.pdf",
  "mimeType": "application/pdf",
  "fileSizeBytes": 102400,
  "requiresValidation": true
}
```

Response (200):
```json
{
  "documentId": "650e8400-e29b-41d4-a716-446655440000",
  "uploadUrl": "https://storage.example.com/upload?token=xyz",
  "expiresAtUtc": "2025-11-20T12:30:00Z"
}
```

**POST** `/api/upload/complete`

Request:
```json
{
  "documentId": "650e8400-e29b-41d4-a716-446655440000",
  "bucketKey": "companies/550e8400/vehicles/VEH-001/soat.pdf"
}
```

### Descarga de Documento

**GET** `/api/download/{documentId}`

Response (200):
```json
{
  "documentId": "650e8400-e29b-41d4-a716-446655440000",
  "fileName": "soat.pdf",
  "mimeType": "application/pdf",
  "downloadUrl": "https://storage.example.com/download?token=abc",
  "fileSizeBytes": 102400
}
```

### Aprobación

**POST** `/api/validation/approve`

Request:
```json
{
  "documentId": "650e8400-e29b-41d4-a716-446655440000",
  "approverUserId": "user-456",
  "reason": "Cumple requisitos"
}
```

Response (200):
```json
{
  "success": true,
  "message": "Documento aprobado"
}
```

### Rechazo

**POST** `/api/validation/reject`

Request:
```json
{
  "documentId": "650e8400-e29b-41d4-a716-446655440000",
  "rejecterUserId": "user-456",
  "reason": "Documento ilegible"
}
```

## 🛡️ Reglas de Negocio

1. **Validación jerárquica**: Mayor `order` aprueba pasos previos
2. **Aprobación completa**: Si el último aprobador aprueba → documento pasa a estado "A"
3. **Rechazo terminal**: Cualquier rechazo pone documento en estado "R" (no reversible)
4. **Atomicidad**: Documento en BD solo si existe en bucket
5. **Auditoría**: Todas las acciones registran actor, razón y timestamp

## 🔐 Consideraciones de Seguridad

- Pre-signed URLs con expiración limitada
- Validación de entrada: tamaños, MIME types
- Registro de auditoría de todas las operaciones
- Control de acceso por empresa
