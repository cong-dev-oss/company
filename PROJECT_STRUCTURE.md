# Cấu trúc dự án - Enterprise Microservices .NET

## Tổng quan

Dự án được tổ chức theo **Clean Architecture** và **Domain-Driven Design**, tuân thủ các best practices của .NET và doanh nghiệp.

## Cấu trúc thư mục chi tiết

### 📂 src/gateway/

**API Gateway** - Entry point cho tất cả requests

```
gateway/
├── ApiGateway/                    # YARP Reverse Proxy
│   ├── Program.cs
│   ├── appsettings.json
│   └── Routes/                    # Route configurations
└── Security/                      # Gateway security layer
    ├── JwtValidator.cs
    ├── OidcConfig.cs
    └── Policies/
```

### 📂 src/services/

Mỗi service tuân theo **Clean Architecture** với các layers:

#### OrderService (Template)

```
OrderService/
├── OrderService.Api/              # Presentation Layer
│   ├── Program.cs
│   ├── Endpoints/                 # Minimal API endpoints
│   │   └── OrdersEndpoints.cs
│   ├── Filters/                   # Exception handlers, action filters
│   └── Middleware/                # Custom middleware
│
├── OrderService.Application/       # Application Layer (Use Cases)
│   ├── Commands/                  # CQRS Commands
│   │   ├── CreateOrder/
│   │   │   ├── CreateOrderCommand.cs
│   │   │   ├── CreateOrderCommandHandler.cs
│   │   │   └── CreateOrderCommandValidator.cs
│   │   └── UpdateOrder/
│   ├── Queries/                   # CQRS Queries
│   │   ├── GetOrderById/
│   │   └── GetOrdersByCustomer/
│   ├── Behaviors/                 # MediatR behaviors
│   ├── Mappings/                  # AutoMapper profiles
│   └── DependencyInjection.cs
│
├── OrderService.Domain/           # Domain Layer (Core Business Logic)
│   ├── Entities/                  # Domain entities
│   │   ├── Order.cs               # Aggregate root
│   │   └── OrderItem.cs
│   ├── ValueObjects/              # Value objects
│   │   ├── Money.cs
│   │   └── Address.cs
│   ├── DomainEvents/              # Domain events
│   │   └── OrderCreatedEvent.cs
│   ├── Enums/
│   ├── Exceptions/                # Domain exceptions
│   └── Interfaces/                # Repository interfaces
│       └── IOrderRepository.cs
│
├── OrderService.Infrastructure/   # Infrastructure Layer
│   ├── Persistence/               # EF Core
│   │   ├── OrderDbContext.cs
│   │   ├── Configurations/        # EF Core configurations
│   │   │   ├── OrderConfiguration.cs
│   │   │   └── OrderItemConfiguration.cs
│   │   └── Migrations/            # Database migrations
│   ├── Repositories/              # Repository implementations
│   │   └── OrderRepository.cs
│   ├── Outbox/                    # Outbox pattern
│   ├── Messaging/                 # Message bus integration
│   └── DependencyInjection.cs
│
├── OrderService.Contracts/        # Contracts Layer
│   ├── DTOs/
│   │   ├── Requests/
│   │   │   └── CreateOrderRequest.cs
│   │   └── Responses/
│   │       └── OrderResponse.cs
│   └── Events/                    # Integration events
│       └── OrderCreatedIntegrationEvent.cs
│
├── OrderService.Security/         # Security Layer
│   ├── Authorization/
│   │   ├── Policies/
│   │   └── Handlers/
│   └── Requirements/
│
├── OrderService.Configuration/    # Configuration Layer
│   ├── Options/
│   ├── HealthChecks/
│   └── DependencyInjection.cs
│
└── OrderService.Tests/            # Tests
    ├── Unit/
    │   ├── Application/
    │   └── Domain/
    ├── Integration/
    │   ├── Api/
    │   └── Infrastructure/
    └── Security/
```

### 📂 src/shared/

**Shared Building Blocks** - Thư viện dùng chung

```
shared/
├── BuildingBlocks/                # Common building blocks
│   ├── Result/                    # Result<T> pattern
│   │   └── Result.cs
│   ├── Guards/                    # Guard clauses
│   │   └── Guard.cs
│   ├── Errors/                    # Error codes
│   └── Extensions/
│
├── Security/                      # Shared security utilities
│   ├── Jwt/
│   ├── Oidc/
│   └── KeyRotation/
│
├── Observability/                 # Observability utilities
│   ├── OpenTelemetry/
│   ├── Logging/
│   └── Metrics/
│
├── Messaging/                     # Messaging infrastructure
│   ├── Kafka/
│   ├── Outbox/
│   └── Inbox/
│
└── Validation/                    # Validation utilities
    ├── FluentValidation/
    └── AntiDoS/
```

## Dependency Rules

### Layer Dependencies

```
┌─────────────────┐
│   Api Layer     │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ Application     │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│   Domain        │  ← Không phụ thuộc layer nào
└────────┬────────┘
         ▲
         │
┌────────┴────────┐
│ Infrastructure │
└────────────────┘
```

**Quy tắc:**
- ✅ **Domain** → Không phụ thuộc gì
- ✅ **Application** → Domain
- ✅ **Infrastructure** → Domain + Application
- ✅ **Api** → Application + Infrastructure + Contracts

## Project References

### OrderService.Api
```xml
<ProjectReference Include="OrderService.Application" />
<ProjectReference Include="OrderService.Infrastructure" />
<ProjectReference Include="OrderService.Contracts" />
<ProjectReference Include="OrderService.Configuration" />
<ProjectReference Include="OrderService.Security" />
```

### OrderService.Application
```xml
<ProjectReference Include="OrderService.Domain" />
<ProjectReference Include="BuildingBlocks" />
```

### OrderService.Infrastructure
```xml
<ProjectReference Include="OrderService.Domain" />
<ProjectReference Include="OrderService.Application" />
```

## Patterns sử dụng

### 1. CQRS (Command Query Responsibility Segregation)
- **Commands**: Thay đổi state (CreateOrder, UpdateOrder, CancelOrder)
- **Queries**: Đọc data (GetOrderById, GetOrdersByCustomer)

### 2. Repository Pattern
- Interface trong **Domain** layer
- Implementation trong **Infrastructure** layer

### 3. Result Pattern
- Type-safe error handling
- Thay thế exceptions cho business errors

### 4. Guard Clauses
- Early validation
- Fail-fast principle

### 5. Value Objects
- Immutable objects (Money, Address)
- Encapsulate business rules

### 6. Domain Events
- Decouple domain logic
- Enable event-driven architecture

## Technology Stack

| Layer | Technology |
|-------|-----------|
| Framework | .NET 9.0 |
| Web | ASP.NET Core (Minimal APIs) |
| ORM | Entity Framework Core 9.0 |
| CQRS | MediatR 12.x |
| Validation | FluentValidation 11.x |
| Logging | Serilog |
| Tracing | OpenTelemetry |
| Messaging | Confluent.Kafka |
| Gateway | YARP |

## Best Practices

1. ✅ **Separation of Concerns**: Mỗi layer có trách nhiệm rõ ràng
2. ✅ **Dependency Inversion**: Domain không phụ thuộc Infrastructure
3. ✅ **Single Responsibility**: Mỗi class có một trách nhiệm
4. ✅ **DRY**: Shared building blocks cho code dùng chung
5. ✅ **SOLID Principles**: Tuân thủ tất cả nguyên tắc SOLID
6. ✅ **Testability**: Dễ test với dependency injection

## Mở rộng

Khi thêm service mới:

1. Copy cấu trúc từ `OrderService` hoặc `InventoryService`
2. Đổi tên namespace và project names
3. Implement domain entities và business logic
4. Cấu hình trong `ApiGateway` routes
5. Thêm vào solution file

## Tài liệu tham khảo

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design](https://www.domainlanguage.com/ddd/)
- [.NET Microservices Architecture](https://dotnet.microsoft.com/learn/aspnet/microservices-architecture)


