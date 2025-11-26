# Start Database Containers Script
Write-Host "🚀 Starting Database Containers..." -ForegroundColor Green
Write-Host ""

# Check if Docker Desktop is running
Write-Host "📋 Checking Docker..." -ForegroundColor Cyan
try {
    $dockerVersion = docker --version 2>&1
    if ($LASTEXITCODE -ne 0) {
        throw "Docker not found"
    }
    Write-Host "  ✅ $dockerVersion" -ForegroundColor Green
} catch {
    Write-Host "  ❌ Docker is not running or not in PATH" -ForegroundColor Red
    Write-Host ""
    Write-Host "  Please:" -ForegroundColor Yellow
    Write-Host "  1. Start Docker Desktop" -ForegroundColor Yellow
    Write-Host "  2. Wait for Docker to be ready" -ForegroundColor Yellow
    Write-Host "  3. Run this script again" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "  Or manually run:" -ForegroundColor Cyan
    Write-Host "    docker compose up -d postgres sqlserver redis pgadmin" -ForegroundColor White
    exit 1
}

# Check Docker Compose
Write-Host ""
Write-Host "📋 Checking Docker Compose..." -ForegroundColor Cyan
try {
    $composeVersion = docker compose version 2>&1
    if ($LASTEXITCODE -ne 0) {
        throw "Docker Compose not found"
    }
    Write-Host "  ✅ $composeVersion" -ForegroundColor Green
} catch {
    Write-Host "  ❌ Docker Compose not available" -ForegroundColor Red
    exit 1
}

# Change to script directory
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptPath

Write-Host ""
Write-Host "📦 Starting containers..." -ForegroundColor Yellow
docker compose up -d postgres sqlserver redis pgadmin

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "❌ Failed to start containers" -ForegroundColor Red
    Write-Host "  Check Docker Desktop is running" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "⏳ Waiting for databases to be ready..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# Check PostgreSQL
Write-Host ""
Write-Host "📊 Checking PostgreSQL..." -ForegroundColor Cyan
$pgReady = docker exec company-postgres pg_isready -U postgres 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "  ✅ PostgreSQL is ready" -ForegroundColor Green
    
    # List databases
    Write-Host ""
    Write-Host "📋 Databases:" -ForegroundColor Cyan
    docker exec company-postgres psql -U postgres -c "\l" 2>&1 | Select-String -Pattern "order_db|inventory_db|identity_db|company_db" | ForEach-Object {
        Write-Host "  $_" -ForegroundColor White
    }
} else {
    Write-Host "  ⚠️  PostgreSQL is starting..." -ForegroundColor Yellow
}

# Check SQL Server
Write-Host ""
Write-Host "📊 Checking SQL Server..." -ForegroundColor Cyan
$sqlReady = docker exec company-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -Q "SELECT 1" 2>&1 | Out-Null
if ($LASTEXITCODE -eq 0) {
    Write-Host "  ✅ SQL Server is ready" -ForegroundColor Green
} else {
    Write-Host "  ⚠️  SQL Server is starting..." -ForegroundColor Yellow
}

# Check Redis
Write-Host ""
Write-Host "📊 Checking Redis..." -ForegroundColor Cyan
$redisReady = docker exec company-redis redis-cli ping 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "  ✅ Redis is ready" -ForegroundColor Green
} else {
    Write-Host "  ⚠️  Redis is starting..." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "✅ Databases are starting!" -ForegroundColor Green
Write-Host ""
Write-Host "📝 Connection Information:" -ForegroundColor Yellow
Write-Host ""
Write-Host "  PostgreSQL:" -ForegroundColor Cyan
Write-Host "    Host: localhost"
Write-Host "    Port: 5432"
Write-Host "    User: postgres"
Write-Host "    Password: postgres"
Write-Host "    Databases: order_db, inventory_db, identity_db"
Write-Host ""
Write-Host "  SQL Server:" -ForegroundColor Cyan
Write-Host "    Host: localhost"
Write-Host "    Port: 1433"
Write-Host "    User: sa"
Write-Host "    Password: YourStrong@Passw0rd"
Write-Host ""
Write-Host "  Redis:" -ForegroundColor Cyan
Write-Host "    Host: localhost"
Write-Host "    Port: 6379"
Write-Host ""
Write-Host "  pgAdmin (Web UI):" -ForegroundColor Cyan
Write-Host "    URL: http://localhost:5050"
Write-Host "    Email: admin@company.com"
Write-Host "    Password: admin"
Write-Host ""
Write-Host "📋 Container Status:" -ForegroundColor Yellow
docker compose ps
Write-Host ""
Write-Host "💡 Next Steps:" -ForegroundColor Green
Write-Host "  1. Wait a few seconds for all databases to be fully ready"
Write-Host "  2. Access pgAdmin at http://localhost:5050"
Write-Host "  3. Update connection strings in appsettings.Development.json"
Write-Host "  4. Run EF Core migrations: dotnet ef database update"
Write-Host ""


