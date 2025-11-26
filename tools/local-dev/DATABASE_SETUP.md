# Database Setup Guide

> **Note:** File này đã được thay thế bởi `README.md` với hướng dẫn đầy đủ và cập nhật hơn.
> Vui lòng xem `README.md` để có hướng dẫn mới nhất.

## Quick Start

### 1. Start Docker Desktop

Đảm bảo Docker Desktop đang chạy trước khi tiếp tục.

### 2. Start Databases

**Option 1: Dùng PowerShell Script (Khuyến nghị)**

```powershell
cd tools\local-dev
powershell -ExecutionPolicy Bypass -File .\start-databases.ps1
```

**Option 2: Dùng Docker Compose trực tiếp**

```cmd
cd tools\local-dev
docker compose up -d postgres sqlserver redis pgadmin
```

### 3. Kiểm tra Database Status

```cmd
docker compose ps
```

### 4. Truy cập pgAdmin (Web UI)

- URL: http://localhost:5050
- Email: `admin@company.com`
- Password: `admin`

### 5. Kết nối PostgreSQL từ pgAdmin

1. Login vào pgAdmin
2. Right-click "Servers" → "Register" → "Server"
3. **General tab:**
   - Name: `Company Local`
4. **Connection tab:**
   - Host name/address: `postgres` (hoặc `host.docker.internal`)
   - Port: `5432`
   - Username: `postgres`
   - Password: `postgres`
   - Save password: ✓
5. Click "Save"

## Connection Strings

### PostgreSQL

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

### SQL Server

```
Server=localhost;Database=OrderServiceDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;
```

## Kiểm tra Databases đã tạo

### Cách 1: Dùng Docker exec

```cmd
# Kiểm tra PostgreSQL
docker exec company-postgres pg_isready -U postgres

# Liệt kê databases
docker exec company-postgres psql -U postgres -c "\l"
```

### Cách 2: Dùng pgAdmin

1. Mở http://localhost:5050
2. Kết nối PostgreSQL (theo hướng dẫn trên)
3. Expand "Company Local" → "Databases"
4. Bạn sẽ thấy:
   - `order_db`
   - `inventory_db`
   - `identity_db`
   - `company_db` (default)

## Run Migrations

### Order Service

```cmd
cd src\services\OrderService\OrderService.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ..\OrderService.Api
dotnet ef database update --startup-project ..\OrderService.Api
```

### Inventory Service

```cmd
cd src\services\InventoryService\InventoryService.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ..\InventoryService.Api
dotnet ef database update --startup-project ..\InventoryService.Api
```

### Identity Service

```cmd
cd src\services\IdentityService\IdentityService.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ..\IdentityService.Api
dotnet ef database update --startup-project ..\IdentityService.Api
```

## Stop Databases

```cmd
cd tools\local-dev
docker compose stop
```

## Remove All Data (Clean Up)

```cmd
cd tools\local-dev
docker compose down -v
```

## Troubleshooting

### Docker không được nhận diện

1. Đảm bảo Docker Desktop đang chạy
2. Restart terminal/PowerShell
3. Hoặc thêm Docker vào PATH:
   - Thường ở: `C:\Program Files\Docker\Docker\resources\bin`

### Port đã được sử dụng

```cmd
# Kiểm tra port nào đang dùng
netstat -ano | findstr :5432

# Hoặc đổi port trong docker-compose.yml
```

### Container không start

```cmd
# Xem logs
docker compose logs postgres

# Restart containers
docker compose restart postgres
```

### Database không được tạo

```cmd
# Xóa volumes và tạo lại
docker compose down -v
docker compose up -d postgres

# Kiểm tra script đã được mount chưa
docker exec company-postgres ls -la /docker-entrypoint-initdb.d/
```

### Kết nối từ code bị lỗi

1. Kiểm tra connection string trong `appsettings.Development.json`
2. Đảm bảo đã thêm NuGet package: `Npgsql.EntityFrameworkCore.PostgreSQL`
3. Đảm bảo code dùng `UseNpgsql()` thay vì `UseSqlServer()`

## Useful Commands

```cmd
# Xem containers đang chạy
docker compose ps

# Xem logs
docker compose logs postgres
docker compose logs -f postgres  # Follow logs

# Restart một service
docker compose restart postgres

# Stop tất cả
docker compose stop

# Remove containers (giữ data)
docker compose down

# Remove containers và data
docker compose down -v

# Rebuild và start
docker compose up -d --build
```


