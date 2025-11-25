# 📚 Swagger/OpenAPI Documentation Guide

Hướng dẫn sử dụng Swagger UI để test và document các API endpoints.

## 🎯 Tổng quan

Tất cả API services đã được tích hợp **Swagger/OpenAPI** với các tính năng:
- ✅ Swagger UI đầy đủ
- ✅ JWT Bearer Authentication support
- ✅ XML Comments documentation
- ✅ Request/Response examples
- ✅ Try it out functionality

## 🔧 Cấu hình

### Packages

Tất cả API projects đã được cấu hình với:
- `Swashbuckle.AspNetCore` 6.9.0
- XML documentation generation
- JWT Bearer authentication trong Swagger

### Shared Configuration

Swagger configuration được tập trung trong `BuildingBlocks.Swagger` để đảm bảo consistency:

```csharp
// Trong mỗi API project
builder.Services.AddSwaggerDocumentation(configuration, "Service Name", "Description");
app.UseSwaggerDocumentation();
```

## 📍 Swagger URLs

Sau khi chạy services, truy cập Swagger UI tại:

### Identity Service
```
https://localhost:5001/swagger
http://localhost:5000/swagger
```

### Order Service
```
https://localhost:5002/swagger
http://localhost:5001/swagger
```

### Inventory Service
```
https://localhost:5003/swagger
http://localhost:5002/swagger
```

### API Gateway
```
https://localhost:5004/swagger
http://localhost:5003/swagger
```

## 🔐 JWT Authentication trong Swagger

### Cách sử dụng

1. **Mở Swagger UI** tại `/swagger`
2. **Click nút "Authorize"** ở góc trên bên phải
3. **Nhập JWT token** theo format: `Bearer {your-token}`
4. **Click "Authorize"** để lưu token
5. **Click "Close"** để đóng dialog

### Lấy JWT Token

**Bước 1: Register hoặc Login**

```bash
# Register
POST /api/auth/register
{
  "email": "test@example.com",
  "password": "Test123!@#",
  "confirmPassword": "Test123!@#",
  "firstName": "Test",
  "lastName": "User"
}

# Hoặc Login
POST /api/auth/login
{
  "email": "test@example.com",
  "password": "Test123!@#"
}
```

**Bước 2: Copy AccessToken từ response**

```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "...",
  "expiresAt": "2024-01-01T12:00:00Z",
  "user": { ... }
}
```

**Bước 3: Sử dụng trong Swagger**

- Click "Authorize"
- Nhập: `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`
- Click "Authorize"

## 🧪 Test API với Swagger

### 1. Test Register Endpoint

1. Mở Swagger UI
2. Tìm endpoint `POST /api/auth/register`
3. Click "Try it out"
4. Nhập request body:
   ```json
   {
     "email": "newuser@example.com",
     "password": "Password123!",
     "confirmPassword": "Password123!",
     "firstName": "New",
     "lastName": "User"
   }
   ```
5. Click "Execute"
6. Xem response

### 2. Test Protected Endpoints

1. **Đăng nhập** để lấy token (như trên)
2. **Authorize** với token trong Swagger
3. **Test protected endpoints** như `GET /api/auth/me`
4. Token sẽ tự động được gửi trong header `Authorization`

## 📝 XML Comments Documentation

### Thêm XML Comments

```csharp
/// <summary>
/// Register a new user
/// </summary>
/// <param name="request">Registration request</param>
/// <returns>Authentication response with tokens</returns>
/// <response code="200">Returns the authentication tokens</response>
/// <response code="400">If the request is invalid</response>
/// <response code="409">If user already exists</response>
[HttpPost("register")]
[ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status409Conflict)]
public async Task<IActionResult> Register([FromBody] RegisterRequest request)
{
    // ...
}
```

### XML Comments sẽ hiển thị trong Swagger

- **Summary**: Mô tả endpoint
- **Parameters**: Mô tả các parameters
- **Responses**: Các response codes và types
- **Examples**: Request/Response examples

## 🎨 Swagger UI Features

### Tính năng đã bật

- ✅ **Display Request Duration**: Hiển thị thời gian request
- ✅ **Deep Linking**: Link trực tiếp đến endpoint
- ✅ **Filter**: Tìm kiếm endpoints
- ✅ **Try It Out**: Test API trực tiếp
- ✅ **Doc Expansion**: Mở rộng/collapse sections
- ✅ **Hide Schemas**: Ẩn schemas mặc định để UI gọn hơn

### Swagger UI Options

Cấu hình trong `SwaggerExtensions.cs`:

```csharp
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Service API v1");
    options.RoutePrefix = "swagger";
    options.DisplayRequestDuration();
    options.EnableDeepLinking();
    options.EnableFilter();
    options.EnableTryItOutByDefault();
    options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
    options.DefaultModelsExpandDepth(-1);
});
```

## 🔄 Workflow Test API

### Luồng test hoàn chỉnh

1. **Mở Identity Service Swagger**: `https://localhost:5001/swagger`

2. **Register User**:
   - `POST /api/auth/register`
   - Copy `accessToken` từ response

3. **Authorize trong Swagger**:
   - Click "Authorize"
   - Nhập: `Bearer {accessToken}`
   - Click "Authorize"

4. **Test Protected Endpoint**:
   - `GET /api/auth/me`
   - Token tự động được gửi
   - Xem user info

5. **Test Refresh Token**:
   - `POST /api/auth/refresh`
   - Nhập expired token và refresh token
   - Nhận token mới

## 📋 Swagger Configuration per Service

### Identity Service

```csharp
services.AddSwaggerDocumentation(
    configuration,
    "Identity Service",
    "Authentication and Authorization Service for Company Microservices"
);
```

### Order Service

```csharp
services.AddSwaggerDocumentation(
    configuration,
    "Order Service",
    "Order Management Service for Company Microservices"
);
```

### Inventory Service

```csharp
services.AddSwaggerDocumentation(
    configuration,
    "Inventory Service",
    "Inventory Management Service for Company Microservices"
);
```

## 🚀 Production Considerations

### Disable Swagger trong Production

Swagger chỉ được enable trong Development environment:

```csharp
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(...);
}
```

### Enable trong Production (nếu cần)

Để enable Swagger trong Production:

```csharp
app.UseSwaggerDocumentation("Service Name", enableInProduction: true);
```

**⚠️ Lưu ý**: Chỉ enable trong Production nếu cần thiết và đã có authentication/authorization đầy đủ.

## 📖 Best Practices

1. **XML Comments**: Luôn thêm XML comments cho tất cả endpoints
2. **Response Types**: Specify `ProducesResponseType` cho mọi response codes
3. **Examples**: Thêm examples cho complex DTOs
4. **Tags**: Sử dụng `[Tags("Category")]` để group endpoints
5. **Descriptions**: Mô tả rõ ràng cho mỗi endpoint

## 🔍 Troubleshooting

### Swagger không hiển thị

1. Kiểm tra environment: Swagger chỉ hiển thị trong Development
2. Kiểm tra route: Truy cập đúng `/swagger`
3. Kiểm tra build: Đảm bảo XML file được generate

### JWT Authentication không work

1. Kiểm tra token format: Phải có prefix `Bearer `
2. Kiểm tra token expiration: Token có thể đã hết hạn
3. Kiểm tra JWT configuration: Issuer, Audience, SecretKey

### XML Comments không hiển thị

1. Kiểm tra `GenerateDocumentationFile` trong `.csproj`
2. Kiểm tra XML file có được generate không
3. Kiểm tra path trong `IncludeXmlComments`

## 📚 Tài liệu tham khảo

- [Swashbuckle.AspNetCore Documentation](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
- [OpenAPI Specification](https://swagger.io/specification/)
- [ASP.NET Core Web API Documentation](https://learn.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger)

## ✨ Features

- ✅ **JWT Bearer Authentication**: Test protected endpoints dễ dàng
- ✅ **XML Documentation**: Tự động generate từ code comments
- ✅ **Request/Response Examples**: Xem examples trực tiếp
- ✅ **Try It Out**: Test API không cần Postman
- ✅ **Schema Explorer**: Xem data models
- ✅ **Response Codes**: Xem tất cả possible responses

---

**Happy Testing! 🚀**


