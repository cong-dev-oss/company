# 🗄️ Local Database Setup Guide

Hướng dẫn setup database local cho dự án Company Microservices sử dụng Docker Desktop.

## ✅ Trạng thái hiện tại

Tất cả services đã được cấu hình và migrations đã được tạo:

| Service | Database | Tables | Status |
|---------|----------|--------|--------|
| **OrderService** | `order_db` | `Orders`, `OrderItems` | ✅ Ready |
| **InventoryService** | `inventory_db` | `Products`, `Stocks` | ✅ Ready |
| **IdentityService** | `identity_db` | `Users`, `Roles`, `RefreshTokens`, Identity tables | ✅ Ready |

**Lưu ý:** Tất cả services đang sử dụng PostgreSQL. Migrations đã được tạo sẵn, chỉ cần apply vào database.

## 📋 Yêu cầu

- Docker Desktop đã cài đặt và đang chạy
- Windows PowerShell hoặc CMD

## 🚀 Quick Start

### Bước 1: Start Docker Desktop

Đảm bảo Docker Desktop đang chạy (icon Docker ở system tray).

### Bước 2: Start Databases

Mở PowerShell hoặc CMD và chạy:

```powershell
cd tools\local-dev
docker compose up -d postgres sqlserver redis pgadmin
```

Hoặc dùng script tự động:

```powershell
cd tools\local-dev
powershell -ExecutionPolicy Bypass -File .\start-databases.ps1
```

### Bước 3: Kiểm tra Status

```cmd
docker compose ps
```

Bạn sẽ thấy các containers:
- ✅ `company-postgres` - PostgreSQL database
- ✅ `company-sqlserver` - SQL Server (Azure SQL Edge)
- ✅ `company-redis` - Redis cache
- ✅ `company-pgadmin` - Web UI để quản lý database

## 📊 Databases đã được tạo tự động

PostgreSQL có sẵn các databases:
- `order_db` - Cho Order Service
- `inventory_db` - Cho Inventory Service
- `identity_db` - Cho Identity Service
- `company_db` - Database mặc định

## 🔗 Connection Information

### PostgreSQL (Database chính)

```
Host: localhost
Port: 5432
Username: postgres
Password: postgres
```

**Connection Strings:**

**Order Service:**
```
Host=localhost;Port=5432;Database=order_db;Username=postgres;Password=postgres
```

**Inventory Service:**
```
Host=localhost;Port=5432;Database=inventory_db;Username=postgres;Password=postgres
```

**Identity Service:**
```
Host=localhost;Port=5432;Database=identity_db;Username=postgres;Password=postgres
```

### SQL Server (Azure SQL Edge)

```
Host: localhost
Port: 1433
Username: sa
Password: YourStrong@Passw0rd
```

**Connection String:**
```
Server=localhost;Database=OrderServiceDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;
```

### Redis

```
Host: localhost
Port: 6379
```

## 🌐 pgAdmin - Web UI

Truy cập pgAdmin để quản lý database qua giao diện web:

- **URL:** http://localhost:5050
- **Email:** `admin@company.com`
- **Password:** `admin`

### Kết nối PostgreSQL từ pgAdmin

1. Mở http://localhost:5050 và đăng nhập
2. Right-click "Servers" → "Register" → "Server"
3. **General tab:**
   - Name: `Company Local`
4. **Connection tab:**
   - Host name/address: `postgres` (hoặc `host.docker.internal`)
   - Port: `5432`
   - Username: `postgres`
   - Password: `postgres`
   - ✅ Save password
5. Click "Save"

Bây giờ bạn có thể xem và quản lý databases qua pgAdmin!

## ✅ Kiểm tra Databases

### Cách 1: Dùng Docker exec

```cmd
# Kiểm tra PostgreSQL sẵn sàng
docker exec company-postgres pg_isready -U postgres

# Liệt kê tất cả databases
docker exec company-postgres psql -U postgres -c "\l"
```

### Cách 2: Dùng pgAdmin

1. Mở pgAdmin (http://localhost:5050)
2. Kết nối PostgreSQL (theo hướng dẫn trên)
3. Expand "Company Local" → "Databases"
4. Bạn sẽ thấy tất cả databases

## 🔄 Run EF Core Migrations

Sau khi databases đã sẵn sàng, apply migrations để tạo tables:

### Order Service

```cmd
cd src\services\OrderService\OrderService.Infrastructure
dotnet ef database update --startup-project ..\OrderService.Api
```

**Database:** `order_db`  
**Tables:** `Orders`, `OrderItems`

### Inventory Service

```cmd
cd src\services\InventoryService\InventoryService.Infrastructure
dotnet ef database update --startup-project ..\InventoryService.Api
```

**Database:** `inventory_db`  
**Tables:** `Products`, `Stocks`

### Identity Service

```cmd
cd src\services\IdentityService\IdentityService.Infrastructure
dotnet ef database update --startup-project ..\IdentityService.Api
```

**Database:** `identity_db`  
**Tables:** `Users`, `Roles`, `RefreshTokens`, `AspNetUserClaims`, `AspNetRoleClaims`, `AspNetUserLogins`, `AspNetUserTokens`, `UserRoles`

### Tạo Migration Mới (Khi cần)

Nếu bạn thay đổi entities và cần tạo migration mới:

```cmd
# Tạo migration mới
cd src\services\ServiceName\ServiceName.Infrastructure
dotnet ef migrations add MigrationName --startup-project ..\ServiceName.Api

# Apply migration
dotnet ef database update --startup-project ..\ServiceName.Api
```

### Xóa Migration (Nếu cần rollback)

```cmd
cd src\services\ServiceName\ServiceName.Infrastructure
dotnet ef migrations remove --startup-project ..\ServiceName.Api
```

## 🛠️ Useful Commands

### Xem containers đang chạy

```cmd
docker compose ps
```

### Xem logs

```cmd
# Xem logs PostgreSQL
docker compose logs postgres

# Xem logs real-time
docker compose logs -f postgres
```

### Restart một service

```cmd
docker compose restart postgres
```

### Stop tất cả databases

```cmd
docker compose stop
```

### Remove containers (giữ data)

```cmd
docker compose down
```

### Remove containers và data (clean up)

```cmd
docker compose down -v
```

## 🐛 Troubleshooting

### Docker không được nhận diện

**Lỗi:** `'docker' is not recognized as an internal or external command`

**Giải pháp:**
1. Đảm bảo Docker Desktop đang chạy
2. Restart terminal/PowerShell
3. Hoặc thêm Docker vào PATH:
   - Thường ở: `C:\Program Files\Docker\Docker\resources\bin`

### Port đã được sử dụng

**Lỗi:** Port 5432, 1433, 6379 đã được sử dụng

**Giải pháp:**
```cmd
# Kiểm tra port nào đang dùng
netstat -ano | findstr :5432

# Hoặc đổi port trong docker-compose.yml
```

### Container không start

**Giải pháp:**
```cmd
# Xem logs để biết lỗi
docker compose logs postgres

# Restart container
docker compose restart postgres
```

### Database không được tạo

**Giải pháp:**
```cmd
# Tạo databases thủ công
docker exec company-postgres psql -U postgres -c "CREATE DATABASE order_db;"
docker exec company-postgres psql -U postgres -c "CREATE DATABASE inventory_db;"
docker exec company-postgres psql -U postgres -c "CREATE DATABASE identity_db;"
```

### Kết nối từ code bị lỗi

**Kiểm tra:**
1. Connection string trong `appsettings.Development.json` đúng chưa?
2. Đã thêm NuGet package `Npgsql.EntityFrameworkCore.PostgreSQL` chưa?
3. Code có dùng `UseNpgsql()` thay vì `UseSqlServer()` chưa?

### Lỗi Migration - "Unable to create DbContext"

**Lỗi:** `The property 'Id' cannot be added to the type 'EntityName' because no property type was specified`

**Giải pháp:**
- Đảm bảo entity có property `Id` hoặc cấu hình shadow property với kiểu rõ ràng:
  ```csharp
  builder.Property<Guid>("Id").ValueGeneratedOnAdd();
  ```

### Lỗi Migration - SQL Server thay vì PostgreSQL

**Lỗi:** Migration tạo cho SQL Server thay vì PostgreSQL

**Giải pháp:**
1. Xóa thư mục `Migrations/` cũ
2. Đảm bảo `DependencyInjection.cs` dùng `UseNpgsql()` không phải `UseSqlServer()`
3. Đảm bảo connection string là PostgreSQL format
4. Tạo lại migration: `dotnet ef migrations add InitialCreate`

## 📝 Next Steps

1. ✅ Databases đã sẵn sàng
2. ✅ Connection strings đã được cấu hình trong `appsettings.Development.json`
3. ✅ EF Core migrations đã được tạo và apply
4. 🚀 Start services và test kết nối:

```cmd
# Order Service
cd src\services\OrderService\OrderService.Api
dotnet run

# Inventory Service
cd src\services\InventoryService\InventoryService.Api
dotnet run

# Identity Service
cd src\services\IdentityService\IdentityService.Api
dotnet run

# API Gateway
cd src\gateway\ApiGateway
dotnet run
```

5. 📊 Kiểm tra services:
   - Order Service: http://localhost:5001/swagger
   - Inventory Service: http://localhost:5002/swagger
   - Identity Service: http://localhost:5003/swagger
   - API Gateway: http://localhost:5000/swagger

## 📚 Tài liệu thêm

- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [pgAdmin Documentation](https://www.pgadmin.org/docs/)

---

**Lưu ý:** File này thay thế cho `DATABASE_SETUP.md` và `fix-sqlserver.ps1` (đã xóa vì không cần thiết).

