# Start All Services Script
Write-Host "🚀 Starting All Services..." -ForegroundColor Green
Write-Host ""

$projectRoot = Split-Path -Parent $MyInvocation.MyCommand.Path

# Check if databases are running
Write-Host "📊 Checking databases..." -ForegroundColor Cyan
$postgresRunning = docker ps --filter "name=company-postgres" --format "{{.Names}}" 2>&1
if ($postgresRunning -notlike "*company-postgres*") {
    Write-Host "  ⚠️  PostgreSQL is not running" -ForegroundColor Yellow
    Write-Host "  💡 Start databases first: cd tools\local-dev && docker compose up -d postgres" -ForegroundColor Cyan
    Write-Host ""
} else {
    Write-Host "  ✅ PostgreSQL is running" -ForegroundColor Green
}

Write-Host ""
Write-Host "🚀 Starting services in separate windows..." -ForegroundColor Yellow
Write-Host ""

# Start Order Service
Write-Host "  📦 Starting Order Service..." -ForegroundColor Cyan
$orderServicePath = Join-Path $projectRoot "src\services\OrderService\OrderService.Api"
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$orderServicePath'; Write-Host '🚀 Order Service' -ForegroundColor Green; dotnet run" -WindowStyle Normal
Start-Sleep -Seconds 2

# Start Inventory Service
Write-Host "  📦 Starting Inventory Service..." -ForegroundColor Cyan
$inventoryServicePath = Join-Path $projectRoot "src\services\InventoryService\InventoryService.Api"
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$inventoryServicePath'; Write-Host '🚀 Inventory Service' -ForegroundColor Green; dotnet run" -WindowStyle Normal
Start-Sleep -Seconds 2

# Start Identity Service
Write-Host "  📦 Starting Identity Service..." -ForegroundColor Cyan
$identityServicePath = Join-Path $projectRoot "src\services\IdentityService\IdentityService.Api"
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$identityServicePath'; Write-Host '🚀 Identity Service' -ForegroundColor Green; dotnet run" -WindowStyle Normal
Start-Sleep -Seconds 2

# Start API Gateway
Write-Host "  📦 Starting API Gateway..." -ForegroundColor Cyan
$gatewayPath = Join-Path $projectRoot "src\gateway\ApiGateway"
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$gatewayPath'; Write-Host '🚀 API Gateway' -ForegroundColor Green; dotnet run" -WindowStyle Normal

Write-Host ""
Write-Host "✅ All services are starting in separate windows" -ForegroundColor Green
Write-Host ""
Write-Host "📍 Service URLs:" -ForegroundColor Yellow
Write-Host ""
Write-Host "  Order Service:" -ForegroundColor Cyan
Write-Host "    URL: http://localhost:5260"
Write-Host "    Swagger: http://localhost:5260/swagger"
Write-Host ""
Write-Host "  Inventory Service:" -ForegroundColor Cyan
Write-Host "    URL: http://localhost:5052"
Write-Host "    Swagger: http://localhost:5052/swagger"
Write-Host ""
Write-Host "  Identity Service:" -ForegroundColor Cyan
Write-Host "    URL: http://localhost:5003"
Write-Host "    Swagger: http://localhost:5003/swagger"
Write-Host ""
Write-Host "  API Gateway:" -ForegroundColor Cyan
Write-Host "    URL: http://localhost:5126"
Write-Host "    Swagger: http://localhost:5126/swagger"
Write-Host ""
Write-Host "💡 Lưu ý:" -ForegroundColor Yellow
Write-Host "  - Mỗi service chạy trong window riêng"
Write-Host "  - Ports có thể khác nhau, kiểm tra launchSettings.json"
Write-Host "  - Nhấn Ctrl+C trong mỗi window để stop service"
Write-Host "  - Đợi vài giây để services khởi động hoàn toàn"
Write-Host ""


