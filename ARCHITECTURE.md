# Enterprise Microservices Architecture - .NET 9.0

## Tổng quan

Kiến trúc microservices doanh nghiệp theo tiêu chuẩn .NET, áp dụng Clean Architecture, Domain-Driven Design (DDD), và các pattern doanh nghiệp.

## Nguyên tắc thiết kế

1. **Clean Architecture**: Tách biệt rõ ràng giữa các layers
2. **Domain-Driven Design**: Domain layer độc lập, không phụ thuộc infrastructure
3. **CQRS**: Tách biệt Command và Query
4. **Event-Driven**: Giao tiếp giữa services qua events
5. **Security-First**: Bảo mật được tích hợp từ đầu
6. **Observability**: Logging, Tracing, Metrics sẵn sàng
7. **Scalability**: Thiết kế để scale theo chiều ngang

## Cấu trúc thư mục

```
Company.Microservices/
├── src/
│   ├── gateway/
│   │   ├── ApiGateway/                    # YARP Reverse Proxy
│   │   │   ├── Program.cs
│   │   │   ├── appsettings.json
│   │   │   └── Routes/                    # Route configurations
│   │   └── Security/                      # Gateway security layer
│   │       ├── Security.csproj
│   │       ├── JwtValidator.cs
│   │       ├── OidcConfig.cs
│   │       └── Policies/
│   │
│   ├── services/
│   │   ├── OrderService/
│   │   │   ├── OrderService.Api/          # Presentation Layer (Minimal APIs/Controllers)
│   │   │   │   ├── Program.cs
│   │   │   │   ├── Endpoints/             # Minimal API endpoints
│   │   │   │   ├── Filters/               # Action filters, exception handlers
│   │   │   │   └── Middleware/            # Custom middleware
│   │   │   │
│   │   │   ├── OrderService.Application/   # Application Layer (Use Cases)
│   │   │   │   ├── Commands/              # CQRS Commands
│   │   │   │   │   ├── CreateOrder/
│   │   │   │   │   │   ├── CreateOrderCommand.cs
│   │   │   │   │   │   ├── CreateOrderCommandHandler.cs
│   │   │   │   │   │   └── CreateOrderCommandValidator.cs
│   │   │   │   │   └── ...
│   │   │   │   ├── Queries/               # CQRS Queries
│   │   │   │   │   ├── GetOrderById/
│   │   │   │   │   │   ├── GetOrderByIdQuery.cs
│   │   │   │   │   │   ├── GetOrderByIdQueryHandler.cs
│   │   │   │   │   │   └── GetOrderByIdQueryValidator.cs
│   │   │   │   │   └── ...
│   │   │   │   ├── Behaviors/             # MediatR behaviors (logging, validation, etc.)
│   │   │   │   ├── Mappings/              # AutoMapper profiles
│   │   │   │   ├── Services/              # Application services
│   │   │   │   └── DependencyInjection.cs
│   │   │   │
│   │   │   ├── OrderService.Domain/        # Domain Layer (Core Business Logic)
│   │   │   │   ├── Entities/              # Domain entities
│   │   │   │   │   ├── Order.cs
│   │   │   │   │   └── OrderItem.cs
│   │   │   │   ├── ValueObjects/          # Value objects
│   │   │   │   │   ├── Money.cs
│   │   │   │   │   └── Address.cs
│   │   │   │   ├── Aggregates/            # Aggregate roots
│   │   │   │   │   └── OrderAggregate.cs
│   │   │   │   ├── DomainEvents/          # Domain events
│   │   │   │   │   └── OrderCreatedEvent.cs
│   │   │   │   ├── Enums/                 # Domain enums
│   │   │   │   ├── Exceptions/            # Domain exceptions
│   │   │   │   └── Interfaces/            # Repository interfaces (không implementation)
│   │   │   │       └── IOrderRepository.cs
│   │   │   │
│   │   │   ├── OrderService.Infrastructure/ # Infrastructure Layer
│   │   │   │   ├── Persistence/           # EF Core, DbContext
│   │   │   │   │   ├── OrderDbContext.cs
│   │   │   │   │   ├── Configurations/    # EF Core configurations
│   │   │   │   │   │   └── OrderConfiguration.cs
│   │   │   │   │   └── Migrations/        # Database migrations
│   │   │   │   ├── Repositories/          # Repository implementations
│   │   │   │   │   └── OrderRepository.cs
│   │   │   │   ├── Outbox/                # Outbox pattern for events
│   │   │   │   │   ├── OutboxMessage.cs
│   │   │   │   │   └── OutboxProcessor.cs
│   │   │   │   ├── Messaging/             # Message bus integration
│   │   │   │   │   └── KafkaPublisher.cs
│   │   │   │   ├── ExternalServices/      # External API clients
│   │   │   │   └── DependencyInjection.cs
│   │   │   │
│   │   │   ├── OrderService.Contracts/     # Contracts Layer (DTOs, Events)
│   │   │   │   ├── DTOs/                  # Data Transfer Objects
│   │   │   │   │   ├── Requests/
│   │   │   │   │   │   └── CreateOrderRequest.cs
│   │   │   │   │   └── Responses/
│   │   │   │   │       └── OrderResponse.cs
│   │   │   │   ├── Events/                # Integration events
│   │   │   │   │   └── OrderCreatedIntegrationEvent.cs
│   │   │   │   └── gRPC/                  # gRPC contracts (nếu có)
│   │   │   │
│   │   │   ├── OrderService.Security/      # Security Layer
│   │   │   │   ├── Authorization/         # Authorization handlers
│   │   │   │   │   ├── Policies/
│   │   │   │   │   │   └── OrderOwnerPolicy.cs
│   │   │   │   │   └── Handlers/
│   │   │   │   │       └── OrderAuthorizationHandler.cs
│   │   │   │   └── Requirements/
│   │   │   │       └── OrderOwnerRequirement.cs
│   │   │   │
│   │   │   ├── OrderService.Configuration/ # Configuration Layer
│   │   │   │   ├── Options/               # Options pattern
│   │   │   │   │   ├── DatabaseOptions.cs
│   │   │   │   │   └── MessagingOptions.cs
│   │   │   │   ├── HealthChecks/          # Health check configurations
│   │   │   │   │   └── DatabaseHealthCheck.cs
│   │   │   │   └── DependencyInjection.cs
│   │   │   │
│   │   │   └── OrderService.Tests/         # Tests
│   │   │       ├── Unit/                  # Unit tests
│   │   │       │   ├── Application/
│   │   │       │   │   └── Commands/
│   │   │       │   └── Domain/
│   │   │       ├── Integration/           # Integration tests
│   │   │       │   ├── Api/
│   │   │       │   │   └── OrderEndpointsTests.cs
│   │   │       │   └── Infrastructure/
│   │   │       │       └── RepositoryTests.cs
│   │   │       └── Security/              # Security tests
│   │   │           └── AuthorizationTests.cs
│   │   │
│   │   └── InventoryService/              # Tương tự OrderService
│   │       └── ...
│   │
│   └── shared/
│       ├── BuildingBlocks/                # Common building blocks
│       │   ├── BuildingBlocks.csproj
│       │   ├── MediatR/                   # MediatR extensions
│       │   ├── Result/                    # Result<T> pattern
│       │   │   └── Result.cs
│       │   ├── Errors/                    # Error codes, error handling
│       │   │   └── ErrorCodes.cs
│       │   ├── Guards/                    # Guard clauses
│       │   │   └── Guard.cs
│       │   └── Extensions/                # Extension methods
│       │
│       ├── Security/                      # Shared security utilities
│       │   ├── Security.csproj
│       │   ├── Jwt/                       # JWT utilities
│       │   │   └── JwtTokenHelper.cs
│       │   ├── Oidc/                      # OIDC configuration
│       │   │   └── OidcOptions.cs
│       │   ├── KeyRotation/               # Key rotation utilities
│       │   └── Certificates/              # Certificate utilities
│       │
│       ├── Observability/                 # Observability utilities
│       │   ├── Observability.csproj
│       │   ├── OpenTelemetry/             # OpenTelemetry setup
│       │   │   └── TelemetryExtensions.cs
│       │   ├── Logging/                   # Structured logging
│       │   │   └── SerilogExtensions.cs
│       │   └── Metrics/                  # Custom metrics
│       │
│       ├── Messaging/                     # Messaging infrastructure
│       │   ├── Messaging.csproj
│       │   ├── Kafka/                     # Kafka integration
│       │   │   ├── KafkaProducer.cs
│       │   │   └── KafkaConsumer.cs
│       │   ├── RabbitMQ/                  # RabbitMQ integration (alternative)
│       │   ├── Outbox/                    # Outbox pattern implementation
│       │   │   └── OutboxService.cs
│       │   ├── Inbox/                     # Inbox pattern implementation
│       │   │   └── InboxService.cs
│       │   └── SchemaRegistry/           # Schema registry integration
│       │
│       └── Validation/                    # Validation utilities
│           ├── Validation.csproj
│           ├── FluentValidation/          # FluentValidation profiles
│           │   └── ValidationExtensions.cs
│           └── AntiDoS/                   # Anti-DoS request limits
│               └── RateLimitingMiddleware.cs
│
├── deploy/
│   ├── k8s/                              # Kubernetes manifests
│   │   ├── namespace.yaml
│   │   ├── api-gateway/
│   │   │   ├── deployment.yaml
│   │   │   ├── service.yaml
│   │   │   └── network-policy.yaml
│   │   ├── order-service/
│   │   │   ├── deployment.yaml
│   │   │   ├── service.yaml
│   │   │   ├── network-policy.yaml
│   │   │   └── rbac.yaml
│   │   └── shared/
│   │       ├── secrets.yaml              # Secret references
│   │       └── configmaps.yaml
│   │
│   ├── bicep_terraform/                  # Infrastructure as Code
│   │   ├── bicep/                        # Azure Bicep
│   │   │   ├── main.bicep
│   │   │   └── modules/
│   │   └── terraform/                    # Terraform (alternative)
│   │       ├── main.tf
│   │       └── modules/
│   │
│   └── manifests/                        # Environment-specific configs
│       ├── development/
│       │   ├── appsettings.Development.json
│       │   └── cors-config.json
│       ├── staging/
│       └── production/
│           ├── appsettings.Production.json
│           └── hsts-config.json
│
├── docs/
│   ├── threat-models/                    # Threat modeling
│   │   ├── stride-analysis.md
│   │   └── owasp-asvs-mapping.md
│   ├── runbooks/                         # Operational runbooks
│   │   ├── security-incident-response.md
│   │   ├── account-lockout-procedure.md
│   │   └── breach-response.md
│   └── compliance/                       # Compliance documentation
│       ├── soc2-controls.md
│       └── iso27001-controls.md
│
└── tools/
    ├── ci-cd/                            # CI/CD pipelines
    │   ├── azure-pipelines.yml
    │   ├── github-actions/
    │   │   └── build-and-test.yml
    │   └── scripts/
    │       ├── sast-scan.sh              # SAST scanning
    │       └── sbom-generate.sh           # SBOM generation
    │
    └── local-dev/                        # Local development tools
        ├── docker-compose.yml            # Local services (DB, Kafka, etc.)
        ├── certificates/                 # Dev certificates
        └── seed-data/                    # Test data scripts
```

## Dependency Rules

### Layer Dependencies (Clean Architecture)

```
Api Layer
  ↓
Application Layer
  ↓
Domain Layer (không phụ thuộc layer nào)
  ↑
Infrastructure Layer
```

**Quy tắc:**
- **Domain**: Không phụ thuộc bất kỳ layer nào, chỉ chứa business logic thuần
- **Application**: Phụ thuộc Domain, định nghĩa use cases
- **Infrastructure**: Phụ thuộc Domain và Application, implement các interface
- **Api**: Phụ thuộc Application và Infrastructure, xử lý HTTP requests

## Technology Stack

### Core
- **.NET 9.0**: Framework chính
- **ASP.NET Core**: Web framework
- **Entity Framework Core**: ORM
- **MediatR**: CQRS pattern implementation
- **AutoMapper**: Object mapping
- **FluentValidation**: Input validation

### Security
- **ASP.NET Core Identity**: Authentication
- **JWT Bearer**: Token-based auth
- **OIDC**: OpenID Connect
- **Policy-based Authorization**: Resource-based authorization

### Messaging
- **Confluent.Kafka**: Kafka client
- **MassTransit**: Message bus abstraction (optional)

### Observability
- **OpenTelemetry**: Distributed tracing
- **Serilog**: Structured logging
- **Prometheus**: Metrics (via OpenTelemetry)

### Testing
- **xUnit**: Test framework
- **Moq**: Mocking framework
- **FluentAssertions**: Assertions
- **Testcontainers**: Integration testing

### Infrastructure
- **YARP**: Reverse proxy (API Gateway)
- **PostgreSQL/SQL Server**: Database
- **Redis**: Caching (optional)
- **Docker**: Containerization
- **Kubernetes**: Orchestration

## Patterns & Practices

### 1. CQRS (Command Query Responsibility Segregation)
- Commands: Thay đổi state (Create, Update, Delete)
- Queries: Đọc data (Get, List, Search)

### 2. Repository Pattern
- Interface trong Domain layer
- Implementation trong Infrastructure layer

### 3. Unit of Work
- Quản lý transaction scope
- Đảm bảo consistency

### 4. Outbox Pattern
- Đảm bảo exactly-once delivery cho events
- Transactional outbox trong database

### 5. Result Pattern
- Thay thế exceptions cho business errors
- Type-safe error handling

### 6. Guard Clauses
- Early validation
- Fail-fast principle

## Security Considerations

1. **Authentication**: JWT/OIDC tại Gateway
2. **Authorization**: Policy-based tại mỗi service
3. **Input Validation**: FluentValidation tại Application layer
4. **Rate Limiting**: Anti-DoS middleware
5. **HTTPS Only**: HSTS headers
6. **CORS**: Configured per environment
7. **Secrets Management**: Azure Key Vault / Kubernetes Secrets
8. **Network Policies**: K8s NetworkPolicies cho service isolation

## Observability

1. **Distributed Tracing**: OpenTelemetry với correlation IDs
2. **Structured Logging**: Serilog với JSON output
3. **Metrics**: Custom metrics + ASP.NET Core metrics
4. **Health Checks**: Database, external services
5. **Application Insights**: Azure monitoring (optional)

## Scalability

1. **Horizontal Scaling**: Stateless services
2. **Database Sharding**: Per-service database
3. **Caching**: Redis cho read-heavy operations
4. **Async Processing**: Background jobs cho heavy operations
5. **Load Balancing**: K8s service load balancing

## Development Workflow

1. **Local Development**: Docker Compose cho dependencies
2. **Testing**: Unit → Integration → E2E
3. **CI/CD**: Automated build, test, security scan, deploy
4. **Code Quality**: SonarQube, CodeQL
5. **Dependency Management**: Dependabot, SBOM generation

