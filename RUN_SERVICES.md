# 🚀 Hướng dẫn Chạy Services

Hướng dẫn chi tiết cách chạy tất cả services trong dự án Company Microservices.

## 📋 Prerequisites

1. ✅ Databases đã được start (xem [Database Setup Guide](./tools/local-dev/README.md))
2. ✅ Migrations đã được apply
3. ✅ .NET 9.0 SDK đã cài đặt

## 🗄️ Bước 1: Start Databases

```cmd
cd tools\local-dev
docker compose up -d postgres sqlserver redis pgadmin
```

Kiểm tra databases đã sẵn sàng:
```cmd
docker compose ps
```

## 🔄 Bước 2: Apply Migrations (Nếu chưa)

```cmd
# Order Service
cd src\services\OrderService\OrderService.Infrastructure
dotnet ef database update --startup-project ..\OrderService.Api

# Inventory Service
cd src\services\InventoryService\InventoryService.Infrastructure
dotnet ef database update --startup-project ..\InventoryService.Api

# Identity Service
cd src\services\IdentityService\IdentityService.Infrastructure
dotnet ef database update --startup-project ..\IdentityService.Api
```

## 🚀 Bước 3: Chạy Services

### Option 1: Chạy từng service trong terminal riêng (Khuyến nghị)

Mở **4 terminal windows** và chạy từng service:

#### Terminal 1: Order Service
```cmd
cd D:\microservices Net\Company\src\services\OrderService\OrderService.Api
dotnet run
```
**URL:** http://localhost:5260  
**Swagger:** http://localhost:5260/swagger

#### Terminal 2: Inventory Service
```cmd
cd D:\microservices Net\Company\src\services\InventoryService\InventoryService.Api
dotnet run
```
**URL:** http://localhost:5052  
**Swagger:** http://localhost:5052/swagger

#### Terminal 3: Identity Service
```cmd
cd D:\microservices Net\Company\src\services\IdentityService\IdentityService.Api
dotnet run
```
**URL:** http://localhost:5003 (hoặc port trong launchSettings.json)  
**Swagger:** http://localhost:5003/swagger

#### Terminal 4: API Gateway
```cmd
cd D:\microservices Net\Company\src\gateway\ApiGateway
dotnet run
```
**URL:** http://localhost:5126  
**Swagger:** http://localhost:5126/swagger

### Option 2: Chạy tất cả bằng script (PowerShell)

Sử dụng script có sẵn:

```powershell
# Từ thư mục root của project
.\start-all-services.ps1
```

Script sẽ tự động:
- Kiểm tra databases đang chạy
- Mở 4 terminal windows riêng
- Chạy từng service trong window riêng
- Hiển thị URLs của tất cả services

**Lưu ý:** Script sẽ mở 4 PowerShell windows mới, mỗi window chạy một service.

## 📍 Service URLs và Ports

| Service | Port | URL | Swagger |
|---------|------|-----|---------|
| **Order Service** | 5260 | http://localhost:5260 | http://localhost:5260/swagger |
| **Inventory Service** | 5052 | http://localhost:5052 | http://localhost:5052/swagger |
| **Identity Service** | 5003 | http://localhost:5003 | http://localhost:5003/swagger |
| **API Gateway** | 5126 | http://localhost:5126 | http://localhost:5126/swagger |

**Lưu ý:** Ports có thể khác nhau tùy theo `launchSettings.json` của mỗi service.

## 🔍 Kiểm tra Services đang chạy

### Health Checks

```cmd
# Order Service
curl http://localhost:5260/health

# Inventory Service
curl http://localhost:5052/health

# Identity Service
curl http://localhost:5003/health

# API Gateway
curl http://localhost:5126/health
```

### Kiểm tra Database Connection

Nếu service không kết nối được database, kiểm tra:
1. Database containers đang chạy: `docker compose ps`
2. Connection string trong `appsettings.Development.json`
3. Logs của service để xem lỗi cụ thể

## 🛑 Stop Services

Nhấn `Ctrl+C` trong mỗi terminal window để stop service.

Hoặc kill process:
```cmd
# Windows
taskkill /F /IM dotnet.exe

# Hoặc kill theo port
netstat -ano | findstr :5260
taskkill /PID <PID> /F
```

## 🔧 Troubleshooting

### Port đã được sử dụng

**Lỗi:** `Failed to bind to address http://localhost:5260`

**Giải pháp:**
1. Tìm process đang dùng port:
   ```cmd
   netstat -ano | findstr :5260
   ```
2. Kill process hoặc đổi port trong `launchSettings.json`

### Service không kết nối được database

**Kiểm tra:**
1. Database containers đang chạy: `docker compose ps`
2. Connection string đúng: `Host=localhost;Port=5432;Database=...`
3. Database đã được tạo: `docker exec company-postgres psql -U postgres -c "\l"`

### Service không start

**Kiểm tra:**
1. Build thành công: `dotnet build`
2. Packages đã restore: `dotnet restore`
3. Xem logs để biết lỗi cụ thể

## 📝 Quick Reference

### Thứ tự chạy services (khuyến nghị)

1. **Databases** (Docker Compose)
2. **Identity Service** (cần cho authentication)
3. **Order Service**
4. **Inventory Service**
5. **API Gateway** (cuối cùng, route đến các services)

### Commands nhanh

```cmd
# Restore packages
dotnet restore

# Build solution
dotnet build

# Run service
cd src\services\ServiceName\ServiceName.Api
dotnet run

# Run với specific profile
dotnet run --launch-profile http
```

## 🎯 Next Steps

Sau khi tất cả services đang chạy:

1. ✅ Test API qua Swagger UI
2. ✅ Test authentication qua Identity Service
3. ✅ Test API Gateway routing
4. ✅ Kiểm tra database connections
5. ✅ Test end-to-end workflows

---

**Xem thêm:**
- [Database Setup Guide](./tools/local-dev/README.md)
- [Architecture Guide](./ARCHITECTURE.md)
- [Swagger Guide](./SWAGGER_GUIDE.md)

