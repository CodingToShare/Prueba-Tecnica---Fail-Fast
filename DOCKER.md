# Erp.Documents - Docker Documentation

## Building and Running with Docker

### Quick Start

```bash
# Build and run all services
docker-compose up -d

# Check services
docker ps

# View logs
docker logs erp-documents-api
```

### Services

- **SQL Server**: localhost:1433
- **API**: http://localhost:8080
- **Swagger**: http://localhost:8080/swagger

### Environment Configuration

Update `docker-compose.yml` for Azure/AWS credentials:

```yaml
environment:
  Storage__Provider: AzureBlob
  Storage__Azure__ConnectionString: "..."
```

### Stopping Services

```bash
# Stop containers
docker-compose down

# Remove volumes
docker-compose down -v
```

### Troubleshooting

1. **Database connection fails**: Ensure SQL Server is healthy
2. **API won''t start**: Check logs with `docker logs erp-documents-api`
3. **Port already in use**: Change ports in docker-compose.yml

For complete Docker documentation, see DOCKER.md in the repository.
