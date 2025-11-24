# 🔐 Authentication & Authorization Guide

Hướng dẫn chi tiết về mô hình Authentication và Authorization trong hệ thống microservices.

## 📋 Tổng quan

Hệ thống sử dụng **JWT (JSON Web Tokens)** cho authentication với các tính năng:
- ✅ JWT Access Token (short-lived, 60 phút)
- ✅ Refresh Token (long-lived, 7 ngày)
- ✅ Role-based Authorization
- ✅ Policy-based Authorization
- ✅ Token refresh mechanism

## 🏗️ Kiến trúc

### IdentityService

**IdentityService** là service chuyên biệt xử lý authentication và authorization:

```
IdentityService/
├── Api/                    # Controllers (AuthController)
├── Application/            # Commands (Register, Login, RefreshToken)
├── Domain/                 # Entities (User, Role, RefreshToken)
├── Infrastructure/         # IdentityDbContext, JWT config
├── Security/              # Authorization policies
└── Contracts/             # DTOs (Requests/Responses)
```

### Shared Security

**Shared Security** library chứa JWT utilities dùng chung:

```
shared/Security/
└── Jwt/
    ├── JwtTokenService.cs  # Generate & validate tokens
    └── JwtTokenHelper.cs   # Helper methods
```

## 🔑 JWT Configuration

### appsettings.json

```json
{
  "Jwt": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "IdentityService",
    "Audience": "CompanyMicroservices",
    "ExpirationMinutes": "60"
  }
}
```

**⚠️ Lưu ý**: Trong production, SecretKey phải:
- Độ dài tối thiểu 32 ký tự
- Lưu trong Azure Key Vault hoặc Kubernetes Secrets
- Không commit vào Git

## 🔄 Authentication Flow

### 1. Register (Đăng ký)

**Endpoint**: `POST /api/auth/register`

**Request**:
```json
{
  "email": "user@example.com",
  "password": "Password123!",
  "confirmPassword": "Password123!",
  "firstName": "John",
  "lastName": "Doe"
}
```

**Response** (200 OK):
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "base64-encoded-random-string",
  "expiresAt": "2024-01-01T12:00:00Z",
  "user": {
    "id": "guid",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "roles": ["User"]
  }
}
```

**Flow**:
1. Validate input (email format, password strength)
2. Check if user already exists
3. Create user với ASP.NET Core Identity
4. Assign default role "User"
5. Generate JWT access token và refresh token
6. Save refresh token vào database
7. Return tokens và user info

### 2. Login (Đăng nhập)

**Endpoint**: `POST /api/auth/login`

**Request**:
```json
{
  "email": "user@example.com",
  "password": "Password123!"
}
```

**Response** (200 OK):
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "base64-encoded-random-string",
  "expiresAt": "2024-01-01T12:00:00Z",
  "user": {
    "id": "guid",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "roles": ["User"]
  }
}
```

**Flow**:
1. Find user by email
2. Check if user is active
3. Verify password với SignInManager
4. Check lockout status
5. Generate tokens
6. Save refresh token
7. Return tokens

**Error Responses**:
- `401 Unauthorized`: Invalid credentials
- `403 Forbidden`: Account locked or inactive
- `400 BadRequest`: Validation errors

### 3. Refresh Token

**Endpoint**: `POST /api/auth/refresh`

**Request**:
```json
{
  "accessToken": "expired-access-token",
  "refreshToken": "valid-refresh-token"
}
```

**Response** (200 OK):
```json
{
  "accessToken": "new-access-token",
  "refreshToken": "new-refresh-token",
  "expiresAt": "2024-01-01T13:00:00Z",
  "user": { ... }
}
```

**Flow**:
1. Validate expired access token (không check lifetime)
2. Extract user ID và JWT ID (jti) từ token
3. Find refresh token trong database
4. Validate refresh token:
   - Not used
   - Not revoked
   - Not expired
   - Matches JWT ID
5. Mark old refresh token as used
6. Generate new tokens
7. Save new refresh token
8. Return new tokens

**Error Responses**:
- `401 Unauthorized`: Invalid/expired tokens

### 4. Get Current User

**Endpoint**: `GET /api/auth/me`

**Headers**:
```
Authorization: Bearer {accessToken}
```

**Response** (200 OK):
```json
{
  "id": "guid",
  "email": "user@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "roles": ["User"]
}
```

## 🛡️ Authorization

### Roles

Hệ thống có 3 roles mặc định:

1. **Admin**: Full access
2. **Manager**: Elevated permissions
3. **User**: Standard user

### Policies

Các authorization policies được định nghĩa trong `IdentityService.Security`:

```csharp
// Require Admin role
[Authorize(Policy = Policies.RequireAdmin)]

// Require User role (hoặc Admin/Manager)
[Authorize(Policy = Policies.RequireUser)]

// Require Manager hoặc Admin
[Authorize(Policy = Policies.RequireManager)]
```

### Sử dụng trong Controllers

```csharp
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    // Require authentication
    [HttpGet]
    [Authorize]
    public IActionResult GetOrders() { ... }

    // Require Admin role
    [HttpDelete("{id}")]
    [Authorize(Policy = Policies.RequireAdmin)]
    public IActionResult DeleteOrder(Guid id) { ... }

    // Require Manager hoặc Admin
    [HttpPut("{id}")]
    [Authorize(Policy = Policies.RequireManager)]
    public IActionResult UpdateOrder(Guid id) { ... }
}
```

## 🔒 Security Best Practices

### 1. Password Requirements

- Minimum 8 characters
- Require uppercase, lowercase, digit, special character
- Configured trong `IdentityOptions`

### 2. Account Lockout

- Max 5 failed attempts
- Lockout duration: 15 minutes
- Enabled by default

### 3. Token Security

- **Access Token**: Short-lived (60 phút)
- **Refresh Token**: Long-lived (7 ngày), stored in database
- **HTTPS Only**: Trong production
- **Token Rotation**: Mỗi lần refresh tạo token mới

### 4. CORS Configuration

```json
{
  "Cors": {
    "AllowedOrigins": "http://localhost:3000,http://localhost:5173"
  }
}
```

## 📝 JWT Claims

Access token chứa các claims:

```json
{
  "sub": "user-id-guid",
  "email": "user@example.com",
  "name": "John Doe",
  "role": "User",
  "role": "Manager",  // Multiple roles possible
  "jti": "jwt-id-guid",
  "iat": 1234567890,
  "exp": 1234571490
}
```

## 🚀 Setup & Usage

### 1. Database Migration

```bash
cd src/services/IdentityService/IdentityService.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../IdentityService.Api
dotnet ef database update --startup-project ../IdentityService.Api
```

### 2. Seed Data

Roles và admin user được tự động seed khi app khởi động (nếu chưa có).

**Default Admin**:
- Email: `admin@company.com`
- Password: `Admin@123!`

### 3. Run Service

```bash
cd src/services/IdentityService/IdentityService.Api
dotnet run
```

Service sẽ chạy tại: `https://localhost:5001` (hoặc port được cấu hình)

### 4. Test với Postman/curl

**Register**:
```bash
curl -X POST https://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test123!@#",
    "confirmPassword": "Test123!@#",
    "firstName": "Test",
    "lastName": "User"
  }'
```

**Login**:
```bash
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test123!@#"
  }'
```

**Get Current User**:
```bash
curl -X GET https://localhost:5001/api/auth/me \
  -H "Authorization: Bearer {accessToken}"
```

**Refresh Token**:
```bash
curl -X POST https://localhost:5001/api/auth/refresh \
  -H "Content-Type: application/json" \
  -d '{
    "accessToken": "{expiredAccessToken}",
    "refreshToken": "{refreshToken}"
  }'
```

## 🔄 Integration với các Services khác

### 1. ApiGateway

ApiGateway cần validate JWT tokens từ IdentityService:

```csharp
// YARP configuration
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://identity-service";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "IdentityService",
            ValidateAudience = true,
            ValidAudience = "CompanyMicroservices",
            ValidateLifetime = true
        };
    });
```

### 2. Other Services

Các services khác (OrderService, InventoryService) validate JWT tokens tương tự:

```csharp
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Same configuration as IdentityService
    });
```

## 📚 Tài liệu tham khảo

- [ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity)
- [JWT Authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/jwt-authn)
- [Authorization Policies](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies)

## ⚠️ Production Checklist

- [ ] Change JWT SecretKey (use Azure Key Vault)
- [ ] Enable HTTPS only
- [ ] Configure CORS properly
- [ ] Enable email confirmation
- [ ] Set up token refresh rotation
- [ ] Implement rate limiting
- [ ] Add logging và monitoring
- [ ] Set up token revocation mechanism
- [ ] Configure token expiration based on security requirements
