-- =====================================================
-- Setup Script: Base de Datos ErpDocuments
-- Database: SQL Server
-- Purpose: Crear BD, tablas y datos iniciales para pruebas
-- =====================================================

-- 1. Crear base de datos
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'ErpDocuments')
BEGIN
    CREATE DATABASE ErpDocuments;
    PRINT 'Base de datos ErpDocuments creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'Base de datos ErpDocuments ya existe.';
END

GO

USE ErpDocuments;

-- 2. Crear esquema si no existe
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'dbo')
BEGIN
    EXEC sp_executesql N'CREATE SCHEMA dbo';
    PRINT 'Esquema dbo creado.';
END

-- 3. Crear tabla Companies
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Companies' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Companies (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        [Name] NVARCHAR(255) NOT NULL,
        CreatedAtUtc DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAtUtc DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
    PRINT 'Tabla Companies creada.';
END

-- 4. Crear tabla DocumentValidationFlows
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DocumentValidationFlows' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.DocumentValidationFlows (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        DocumentId UNIQUEIDENTIFIER NOT NULL,
        [Status] INT NULL,
        CreatedAtUtc DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAtUtc DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
    PRINT 'Tabla DocumentValidationFlows creada.';
END

-- 5. Crear tabla Documents
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Documents' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Documents (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        CompanyId UNIQUEIDENTIFIER NOT NULL,
        EntityType NVARCHAR(100) NOT NULL,
        EntityId NVARCHAR(255) NOT NULL,
        [Name] NVARCHAR(512) NOT NULL,
        MimeType NVARCHAR(100) NOT NULL,
        SizeBytes BIGINT NOT NULL,
        BucketKey NVARCHAR(1024) NOT NULL UNIQUE,
        Hash NVARCHAR(256) NULL,
        CreatedByUserId NVARCHAR(255) NOT NULL,
        CreatedAtUtc DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAtUtc DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        ValidationStatus INT NULL,
        ValidationFlowId UNIQUEIDENTIFIER NULL,
        CONSTRAINT FK_Documents_Companies FOREIGN KEY (CompanyId) 
            REFERENCES dbo.Companies(Id) ON DELETE CASCADE,
        CONSTRAINT FK_Documents_ValidationFlow FOREIGN KEY (ValidationFlowId) 
            REFERENCES dbo.DocumentValidationFlows(Id) ON DELETE SET NULL
    );
    
    CREATE NONCLUSTERED INDEX IX_Documents_BucketKey ON dbo.Documents(BucketKey);
    CREATE NONCLUSTERED INDEX IX_Documents_CompanyId ON dbo.Documents(CompanyId);
    CREATE NONCLUSTERED INDEX IX_Documents_CompanyEntityId ON dbo.Documents(CompanyId, EntityType, EntityId);
    
    PRINT 'Tabla Documents creada con índices.';
END

-- 6. Crear tabla ValidationSteps
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ValidationSteps' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.ValidationSteps (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        FlowId UNIQUEIDENTIFIER NOT NULL,
        [Order] INT NOT NULL,
        ApproverUserId NVARCHAR(255) NOT NULL,
        [Status] INT NOT NULL,
        CompletedAtUtc DATETIME2 NULL,
        CreatedAtUtc DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAtUtc DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_ValidationSteps_Flow FOREIGN KEY (FlowId) 
            REFERENCES dbo.DocumentValidationFlows(Id) ON DELETE CASCADE
    );
    PRINT 'Tabla ValidationSteps creada.';
END

-- 7. Crear tabla ValidationActions
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ValidationActions' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.ValidationActions (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        FlowId UNIQUEIDENTIFIER NOT NULL,
        StepId UNIQUEIDENTIFIER NULL,
        ActionType INT NOT NULL,
        ActorUserId NVARCHAR(255) NOT NULL,
        [Reason] NVARCHAR(1024) NULL,
        CreatedAtUtc DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_ValidationActions_Flow FOREIGN KEY (FlowId) 
            REFERENCES dbo.DocumentValidationFlows(Id) ON DELETE CASCADE,
        CONSTRAINT FK_ValidationActions_Step FOREIGN KEY (StepId) 
            REFERENCES dbo.ValidationSteps(Id)
    );
    PRINT 'Tabla ValidationActions creada.';
END

-- 8. Crear tabla EF Core Migrations (required for EF Core)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '__EFMigrationsHistory' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.__EFMigrationsHistory (
        MigrationId NVARCHAR(150) NOT NULL PRIMARY KEY,
        ProductVersion NVARCHAR(32) NOT NULL
    );
    
    -- Registrar la migración inicial
    INSERT INTO dbo.__EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20251120112154_InitialCreate', '10.0.0');
    
    PRINT 'Tabla __EFMigrationsHistory creada y migración registrada.';
END

GO

-- =====================================================
-- INSERTAR DATOS DE PRUEBA
-- =====================================================

PRINT '';
PRINT '=== Insertando datos de prueba ===';

-- 9. Insertar companías
DECLARE @CompanyId1 UNIQUEIDENTIFIER = '550e8400-e29b-41d4-a716-446655440000'
DECLARE @CompanyId2 UNIQUEIDENTIFIER = '550e8400-e29b-41d4-a716-446655440001'

-- Limpiar datos existentes (opcional)
-- DELETE FROM dbo.Documents WHERE CompanyId IN (@CompanyId1, @CompanyId2)
-- DELETE FROM dbo.Companies WHERE Id IN (@CompanyId1, @CompanyId2)

IF NOT EXISTS (SELECT 1 FROM dbo.Companies WHERE Id = @CompanyId1)
BEGIN
    INSERT INTO dbo.Companies (Id, [Name], CreatedAtUtc, UpdatedAtUtc)
    VALUES (@CompanyId1, 'TransportCo S.A.', GETUTCDATE(), GETUTCDATE());
    PRINT 'Compañía 1 (TransportCo S.A.) insertada.';
END

IF NOT EXISTS (SELECT 1 FROM dbo.Companies WHERE Id = @CompanyId2)
BEGIN
    INSERT INTO dbo.Companies (Id, [Name], CreatedAtUtc, UpdatedAtUtc)
    VALUES (@CompanyId2, 'LogisticaGlobal Ltd.', GETUTCDATE(), GETUTCDATE());
    PRINT 'Compañía 2 (LogisticaGlobal Ltd.) insertada.';
END

-- 10. Insertar documentos SIN validación (para prueba 1)
DECLARE @DocId1 UNIQUEIDENTIFIER = '650e8400-e29b-41d4-a716-446655440000'

IF NOT EXISTS (SELECT 1 FROM dbo.Documents WHERE Id = @DocId1)
BEGIN
    INSERT INTO dbo.Documents 
        (Id, CompanyId, EntityType, EntityId, [Name], MimeType, SizeBytes, BucketKey, 
         CreatedByUserId, ValidationStatus, ValidationFlowId)
    VALUES (
        @DocId1,
        @CompanyId1,
        'vehicle',
        'VEH-001',
        'vehicle-photo.jpg',
        'image/jpeg',
        512000,
        'companies/550e8400/vehicles/VEH-001/vehicle-photo.jpg',
        'user-sebastian-123',
        NULL,  -- Sin validación
        NULL
    );
    PRINT 'Documento 1 (sin validación) insertado.';
END

-- 11. Insertar documentos CON validación (para prueba 2-3)
DECLARE @DocId2 UNIQUEIDENTIFIER = '660e8400-e29b-41d4-a716-446655440001'
DECLARE @FlowId1 UNIQUEIDENTIFIER = '750e8400-e29b-41d4-a716-446655440000'

IF NOT EXISTS (SELECT 1 FROM dbo.DocumentValidationFlows WHERE Id = @FlowId1)
BEGIN
    INSERT INTO dbo.DocumentValidationFlows (Id, DocumentId, [Status], CreatedAtUtc, UpdatedAtUtc)
    VALUES (@FlowId1, @DocId2, 0, GETUTCDATE(), GETUTCDATE()); -- Status: Pending
    PRINT 'Flujo de validación 1 creado.';
END

IF NOT EXISTS (SELECT 1 FROM dbo.Documents WHERE Id = @DocId2)
BEGIN
    INSERT INTO dbo.Documents 
        (Id, CompanyId, EntityType, EntityId, [Name], MimeType, SizeBytes, BucketKey, 
         CreatedByUserId, ValidationStatus, ValidationFlowId)
    VALUES (
        @DocId2,
        @CompanyId1,
        'vehicle',
        'VEH-002',
        'soat-certificate.pdf',
        'application/pdf',
        256000,
        'companies/550e8400/vehicles/VEH-002/soat-certificate.pdf',
        'user-sebastian-123',
        0,  -- Status: Pending
        @FlowId1
    );
    PRINT 'Documento 2 (con validación) insertado.';
END

-- 12. Insertar pasos de validación jerárquica (3 niveles)
DECLARE @StepId1 UNIQUEIDENTIFIER = NEWID()
DECLARE @StepId2 UNIQUEIDENTIFIER = NEWID()
DECLARE @StepId3 UNIQUEIDENTIFIER = NEWID()

IF NOT EXISTS (SELECT 1 FROM dbo.ValidationSteps WHERE FlowId = @FlowId1 AND [Order] = 1)
BEGIN
    INSERT INTO dbo.ValidationSteps (Id, FlowId, [Order], ApproverUserId, [Status], CreatedAtUtc, UpdatedAtUtc)
    VALUES (@StepId1, @FlowId1, 1, 'user-sebastian-123', 0, GETUTCDATE(), GETUTCDATE());
    
    INSERT INTO dbo.ValidationSteps (Id, FlowId, [Order], ApproverUserId, [Status], CreatedAtUtc, UpdatedAtUtc)
    VALUES (@StepId2, @FlowId1, 2, 'user-camilo-456', 0, GETUTCDATE(), GETUTCDATE());
    
    INSERT INTO dbo.ValidationSteps (Id, FlowId, [Order], ApproverUserId, [Status], CreatedAtUtc, UpdatedAtUtc)
    VALUES (@StepId3, @FlowId1, 3, 'user-juan-789', 0, GETUTCDATE(), GETUTCDATE());
    
    PRINT 'Pasos de validación jerárquica (3 niveles) insertados.';
END

-- 13. Documento para prueba de rechazo
DECLARE @DocId3 UNIQUEIDENTIFIER = '670e8400-e29b-41d4-a716-446655440002'
DECLARE @FlowId2 UNIQUEIDENTIFIER = '750e8400-e29b-41d4-a716-446655440001'

IF NOT EXISTS (SELECT 1 FROM dbo.DocumentValidationFlows WHERE Id = @FlowId2)
BEGIN
    INSERT INTO dbo.DocumentValidationFlows (Id, DocumentId, [Status], CreatedAtUtc, UpdatedAtUtc)
    VALUES (@FlowId2, @DocId3, 0, GETUTCDATE(), GETUTCDATE());
END

IF NOT EXISTS (SELECT 1 FROM dbo.Documents WHERE Id = @DocId3)
BEGIN
    INSERT INTO dbo.Documents 
        (Id, CompanyId, EntityType, EntityId, [Name], MimeType, SizeBytes, BucketKey, 
         CreatedByUserId, ValidationStatus, ValidationFlowId)
    VALUES (
        @DocId3,
        @CompanyId1,
        'employee',
        'EMP-001',
        'invalid-certificate.pdf',
        'application/pdf',
        128000,
        'companies/550e8400/employees/EMP-001/invalid-certificate.pdf',
        'user-sebastian-123',
        0,  -- Status: Pending
        @FlowId2
    );
    PRINT 'Documento 3 (para prueba de rechazo) insertado.';
END

GO

-- =====================================================
-- VERIFICACIÓN FINAL
-- =====================================================

PRINT '';
PRINT '=== Verificación de tablas ===';

PRINT '';
PRINT 'Compañías:';
SELECT Id, [Name], CreatedAtUtc FROM dbo.Companies;

PRINT '';
PRINT 'Documentos:';
SELECT Id, CompanyId, EntityType, EntityId, [Name], ValidationStatus FROM dbo.Documents;

PRINT '';
PRINT 'Flujos de validación:';
SELECT Id, DocumentId, [Status] FROM dbo.DocumentValidationFlows;

PRINT '';
PRINT 'Pasos de validación:';
SELECT Id, FlowId, [Order], ApproverUserId, [Status] FROM dbo.ValidationSteps;

PRINT '';
PRINT '=== Setup completado exitosamente ===';
