# Erp.Documents - API Documentation

## Overview

REST API para gestión de documentos en un ERP con validación jerárquica multi-paso.

**Base URL**: `http://localhost:8080/api`

## Endpoints

### 1. Initiate Upload
POST /api/upload/initiate

**Request**:
```json
{
  "companyId": "guid",
  "entityType": "Invoice",
  "entityId": "INV-001",
  "fileName": "invoice.pdf",
  "mimeType": "application/pdf",
  "fileSizeBytes": 102400,
  "requiresValidation": true
}
```

**Response** (200):
```json
{
  "documentId": "guid",
  "uploadUrl": "https://...",
  "expiresAtUtc": "2025-11-20T12:30:00Z"
}
```

### 2. Complete Upload
POST /api/upload/complete

**Request**:
```json
{
  "documentId": "guid",
  "bucketKey": "documents/.../file.pdf"
}
```

**Response** (200):
```json
{
  "documentId": "guid",
  "downloadUrl": "https://...",
  "expiresAtUtc": "2025-11-21T12:30:00Z"
}
```

### 3. Download Document
GET /api/download/{documentId}

**Response** (200):
```json
{
  "documentId": "guid",
  "fileName": "invoice.pdf",
  "mimeType": "application/pdf",
  "downloadUrl": "https://...",
  "fileSizeBytes": 102400
}
```

### 4. Approve Document
POST /api/validation/approve

**Request**:
```json
{
  "documentId": "guid",
  "approverUserId": "user-456",
  "reason": "Revisado correctamente"
}
```

**Response** (200):
```json
{
  "documentId": "guid",
  "status": "Approved",
  "message": "Documento aprobado",
  "success": true
}
```

### 5. Reject Document
POST /api/validation/reject

**Request**:
```json
{
  "documentId": "guid",
  "rejecterUserId": "user-456",
  "reason": "Faltan datos"
}
```

### 6. Get Validation Status
GET /api/validation/{documentId}/status

**Response** (200):
```json
{
  "documentId": "guid",
  "currentStatus": "Pending",
  "validationFlow": {
    "steps": [...],
    "actions": [...]
  }
}
```

## Validation Rules
- FileName: Required, max 255 chars
- FileSizeBytes: > 0
- MimeType: Valid format (e.g., application/pdf)
- ApproverUserId: Required, max 100 chars
- Reason: Optional, max 500 chars

For detailed documentation, see API.md in the repository.
