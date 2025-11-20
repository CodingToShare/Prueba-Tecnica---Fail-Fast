# Erp.Documents - Developer Guide

## Prerequisites

- .NET 10.0 SDK
- SQL Server (local or Docker)
- Visual Studio 2022 or VS Code

## Project Setup

### 1. Clone Repository

```bash
git clone https://github.com/CodingToShare/Prueba-Tecnica---Fail-Fast.git
cd "Prueba Tecnica - Fail Fast"
```

### 2. Restore Packages

```bash
dotnet restore
```

### 3. Database Setup

```bash
cd Erp.Documents.Infrastructure
dotnet ef database update
```

### 4. Run API

```bash
cd ../Erp.Documents.Api
dotnet run --launch-profile https
```

Visit: https://localhost:5001/swagger

### 5. Run Tests

```bash
cd ../Erp.Documents.Tests
dotnet test
```

## Project Structure

```
Erp.Documents.Api/              # REST API & Controllers
Erp.Documents.Application/      # DTOs, Services, Validators
Erp.Documents.Domain/           # Entities, Enums
Erp.Documents.Infrastructure/   # Data Access, Storage
Erp.Documents.Tests/            # Unit & Integration Tests
```

## Development Workflow

### Building

```bash
dotnet build
```

### Debugging (Visual Studio)

- Press F5 to debug
- Set breakpoints in code

### Debugging (VS Code)

- Install C# extension
- Press F5 to debug

### Running with Watch Mode

```bash
dotnet watch run
```

## Database Commands

```bash
# Create migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Remove migration
dotnet ef migrations remove
```

## Code Style

- **Classes, Methods**: PascalCase
- **Private fields**: _camelCase
- **Local variables**: camelCase

## Git Workflow

```bash
# Create branch
git checkout -b feature/your-feature

# Make changes
git add .
git commit -m "feat: Your feature"

# Push
git push origin feature/your-feature
```

For complete developer guide, see DEVELOPER.md in the repository.
