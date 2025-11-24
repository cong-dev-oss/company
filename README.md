# Company Microservices - Enterprise .NET Architecture

Kiến trúc microservices doanh nghiệp theo tiêu chuẩn .NET 9.0, áp dụng Clean Architecture, Domain-Driven Design (DDD), và các pattern doanh nghiệp.

## 🏗️ Kiến trúc

Dự án tuân theo **Clean Architecture** với các nguyên tắc:

- **Domain Layer**: Core business logic, không phụ thuộc bất kỳ layer nào
- **Application Layer**: Use cases, CQRS với MediatR
- **Infrastructure Layer**: EF Core, repositories, external services
- **Api Layer**: Controllers/Minimal APIs, input validation

Xem chi tiết trong [ARCHITECTURE.md](./ARCHITECTURE.md)

## 📁 Cấu trúc dự án

```
Company.Microservices/
├── src/
│   ├── gateway/              # API Gateway (YARP)
│   ├── services/             # Microservices
│   │   ├── OrderService/
│   │   └── InventoryService/
│   └── shared/               # Shared libraries
│       ├── BuildingBlocks/   # Result pattern, Guards, etc.
│       ├── Security/         # JWT, OIDC utilities
│       ├── Observability/    # OpenTelemetry, logging
│       ├── Messaging/        # Kafka, outbox pattern
│       └── Validation/       # FluentValidation extensions
├── deploy/                   # Deployment configs
├── docs/                     # Documentation
└── tools/                    # CI/CD, local dev tools
```

## 🚀 Bắt đầu

### Yêu cầu

- .NET 9.0 SDK
- Docker Desktop (cho local development)
- PostgreSQL hoặc SQL Server

### Chạy local

1. **Restore packages:**
   ```bash
   dotnet restore
   ```

2. **Build solution:**
   ```bash
   dotnet build
   ```

3. **Run services:**
   ```bash
   # Order Service
   cd src/services/OrderService/OrderService.Api
   dotnet run

   # Inventory Service
   cd src/services/InventoryService/InventoryService.Api
   dotnet run

   # API Gateway
   cd src/gateway/ApiGateway
   dotnet run
   ```

### Docker Compose (Local Development)

```bash
docker-compose -f tools/local-dev/docker-compose.yml up -d
```

## 📦 Packages chính

- **MediatR**: CQRS pattern
- **FluentValidation**: Input validation
- **Entity Framework Core**: ORM
- **YARP**: Reverse proxy cho API Gateway
- **OpenTelemetry**: Distributed tracing
- **Serilog**: Structured logging

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test src/services/OrderService/OrderService.Tests
```

## 🔒 Security

- JWT/OIDC authentication tại Gateway
- Policy-based authorization tại mỗi service
- Input validation với FluentValidation
- Rate limiting và anti-DoS protection

## 📊 Observability

- **Distributed Tracing**: OpenTelemetry
- **Structured Logging**: Serilog
- **Metrics**: ASP.NET Core metrics + Prometheus
- **Health Checks**: Database, external services

## 🏭 Deployment

Xem chi tiết trong `deploy/`:
- Kubernetes manifests
- Infrastructure as Code (Bicep/Terraform)
- Environment-specific configs

## 📚 Documentation

- [ARCHITECTURE.md](./ARCHITECTURE.md) - Kiến trúc chi tiết
- `docs/threat-models/` - Threat modeling
- `docs/runbooks/` - Operational runbooks
- `docs/compliance/` - Compliance documentation

## 🤝 Contributing

1. Tạo feature branch từ `main`
2. Implement theo Clean Architecture principles
3. Viết tests (Unit + Integration)
4. Submit PR với description rõ ràng

## 📄 License

[Your License Here]

