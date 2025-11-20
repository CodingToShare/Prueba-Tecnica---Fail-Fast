# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["Erp.Documents.sln", "."]
COPY ["Erp.Documents.Api/Erp.Documents.Api.csproj", "Erp.Documents.Api/"]
COPY ["Erp.Documents.Application/Erp.Documents.Application.csproj", "Erp.Documents.Application/"]
COPY ["Erp.Documents.Domain/Erp.Documents.Domain.csproj", "Erp.Documents.Domain/"]
COPY ["Erp.Documents.Infrastructure/Erp.Documents.Infrastructure.csproj", "Erp.Documents.Infrastructure/"]

# Restore NuGet packages
RUN dotnet restore "Erp.Documents.sln"

# Copy source code
COPY . .

# Build the application
WORKDIR "/src/Erp.Documents.Api"
RUN dotnet build "Erp.Documents.Api.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "Erp.Documents.Api.csproj" -c Release -o /app/publish

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published application
COPY --from=publish /app/publish .

# Expose port
EXPOSE 8080
EXPOSE 8081

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080;https://+:8081
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the application
ENTRYPOINT ["dotnet", "Erp.Documents.Api.dll"]
