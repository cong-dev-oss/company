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
- PostgreSQL (database chính) - được setup tự động qua Docker Compose

### ✅ Trạng thái Setup

Tất cả services đã được cấu hình sẵn:
- ✅ **OrderService**: Database `order_db`, migrations đã tạo
- ✅ **InventoryService**: Database `inventory_db`, migrations đã tạo  
- ✅ **IdentityService**: Database `identity_db`, migrations đã tạo
- ✅ **API Gateway**: Đã cấu hình YARP

**Next:** Chỉ cần start databases và apply migrations (xem bên dưới)

### Chạy local

1. **Setup Database (Bước đầu tiên):**
   
   Xem hướng dẫn chi tiết: [Database Setup Guide](./tools/local-dev/README.md)
   
   ```bash
   # Start databases với Docker Compose
   cd tools/local-dev
   docker compose up -d postgres sqlserver redis pgadmin
   ```
   
   Databases sẽ được tạo tự động:
   - `order_db` - Cho Order Service
   - `inventory_db` - Cho Inventory Service  
   - `identity_db` - Cho Identity Service
   
   **pgAdmin (Web UI):** http://localhost:5050
   - Email: `admin@company.com`
   - Password: `admin`

2. **Run EF Core Migrations:**
   
   Tất cả migrations đã được tạo sẵn. Chỉ cần apply vào database:
   
   ```bash
   # Order Service
   cd src/services/OrderService/OrderService.Infrastructure
   dotnet ef database update --startup-project ..\OrderService.Api
   
   # Inventory Service
   cd src/services/InventoryService/InventoryService.Infrastructure
   dotnet ef database update --startup-project ..\InventoryService.Api
   
   # Identity Service
   cd src/services/IdentityService/IdentityService.Infrastructure
   dotnet ef database update --startup-project ..\IdentityService.Api
   ```
   
   **Lưu ý:** Nếu cần tạo migration mới:
   ```bash
   dotnet ef migrations add MigrationName --startup-project ..\ServiceName.Api
   ```

3. **Restore packages:**
   ```bash
   dotnet restore
   ```

4. **Build solution:**
   ```bash
   dotnet build
   ```

5. **Run services:**
   ```bash
   # Order Service
   cd src/services/OrderService/OrderService.Api
   dotnet run

   # Inventory Service
   cd src/services/InventoryService/InventoryService.Api
   dotnet run

   # Identity Service
   cd src/services/IdentityService/IdentityService.Api
   dotnet run

   # API Gateway
   cd src/gateway/ApiGateway
   dotnet run
   ```

## 📦 Packages chính

- **MediatR**: CQRS pattern
- **FluentValidation**: Input validation
- **Entity Framework Core**: ORM
- **Npgsql.EntityFrameworkCore.PostgreSQL**: PostgreSQL provider cho EF Core (tất cả services)
- **Microsoft.AspNetCore.Identity.EntityFrameworkCore**: Identity management (IdentityService)
- **YARP**: Reverse proxy cho API Gateway
- **OpenTelemetry**: Distributed tracing
- **Serilog**: Structured logging

## 🗄️ Database Status

Tất cả services đã được cấu hình với PostgreSQL:

- **OrderService**: Database `order_db` - Tables: `Orders`, `OrderItems`
- **InventoryService**: Database `inventory_db` - Tables: `Products`, `Stocks`
- **IdentityService**: Database `identity_db` - Tables: `Users`, `Roles`, `RefreshTokens`, và các Identity tables

Xem chi tiết: [Database Setup Guide](./tools/local-dev/README.md)

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

### Local Development

- **Docker Compose**: Setup databases và infrastructure services
  - PostgreSQL, SQL Server (Azure SQL Edge), Redis, Kafka, pgAdmin
  - Xem: [tools/local-dev/README.md](./tools/local-dev/README.md)

### Production

Xem chi tiết trong `deploy/`:
- Kubernetes manifests
- Infrastructure as Code (Bicep/Terraform)
- Environment-specific configs

## 📚 Documentation

- [ARCHITECTURE.md](./ARCHITECTURE.md) - Kiến trúc chi tiết
- [Database Setup Guide](./tools/local-dev/README.md) - Hướng dẫn setup database local với Docker
- `docs/threat-models/` - Threat modeling
- `docs/runbooks/` - Operational runbooks
- `docs/compliance/` - Compliance documentation

## 🤝 Contributing

1. Tạo feature branch từ `main`
2. Implement theo Clean Architecture principles
3. Viết tests (Unit + Integration)
4. Submit PR với description rõ ràng

## ✅ Setup Checklist

Sau khi clone repository, thực hiện các bước sau:

- [ ] Cài đặt .NET 9.0 SDK
- [ ] Cài đặt và start Docker Desktop
- [ ] Start databases: `cd tools/local-dev && docker compose up -d postgres sqlserver redis pgadmin`
- [ ] Apply migrations cho tất cả services (xem phần "Run EF Core Migrations" ở trên)
- [ ] Restore packages: `dotnet restore`
- [ ] Build solution: `dotnet build`
- [ ] Start services và test

Xem hướng dẫn chi tiết: [Database Setup Guide](./tools/local-dev/README.md)

## 📄 License

[Your License Here]

