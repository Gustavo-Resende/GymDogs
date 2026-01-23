Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  GymDogs - Docker Setup & Migration" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if .env exists
if (-not (Test-Path .env)) {
    Write-Host "Creating .env file from env.example..." -ForegroundColor Yellow
    if (Test-Path env.example) {
        Copy-Item env.example .env
        Write-Host ".env file created. Please review and update secrets if needed!" -ForegroundColor Yellow
    } else {
        Write-Host "Warning: env.example not found. Creating .env with default values..." -ForegroundColor Yellow
        @"
POSTGRES_DB=GymDogsDb
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres
PGADMIN_EMAIL=admin@gymdogs.com
PGADMIN_PASSWORD=admin
JWT_SECRET_KEY=SuaChaveSecretaSuperSeguraComPeloMenos32CaracteresParaHS256
JWT_ISSUER=GymDogs
JWT_AUDIENCE=GymDogsUsers
JWT_ACCESS_TOKEN_EXPIRATION_MINUTES=15
JWT_REFRESH_TOKEN_EXPIRATION_DAYS=7
"@ | Out-File -FilePath .env -Encoding utf8
    }
    Write-Host ""
}

# Load .env file
if (Test-Path .env) {
    Get-Content .env | ForEach-Object {
        if ($_ -match '^\s*([^#][^=]+)=(.*)$') {
            $key = $matches[1].Trim()
            $value = $matches[2].Trim()
            if (-not [string]::IsNullOrEmpty($key)) {
                [Environment]::SetEnvironmentVariable($key, $value, "Process")
            }
        }
    }
}

# Start containers
Write-Host "Starting Docker containers..." -ForegroundColor Green
docker-compose up -d

if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to start containers!" -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "Waiting for PostgreSQL to be ready..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

# Wait for PostgreSQL to be healthy
$maxAttempts = 30
$attempt = 0
do {
    $health = docker-compose exec -T postgres pg_isready -U postgres 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "PostgreSQL is ready!" -ForegroundColor Green
        break
    }
    $attempt++
    Write-Host "Waiting for PostgreSQL... ($attempt/$maxAttempts)" -ForegroundColor Yellow
    Start-Sleep -Seconds 2
} while ($attempt -lt $maxAttempts)

if ($attempt -eq $maxAttempts) {
    Write-Host "PostgreSQL failed to become ready!" -ForegroundColor Red
    exit 1
}

# Run migrations
Write-Host ""
Write-Host "Running database migrations..." -ForegroundColor Green
$networkName = "gymdogs_gymdogs-network"
$dbName = $env:POSTGRES_DB
if ([string]::IsNullOrEmpty($dbName)) { $dbName = "GymDogsDb" }
$dbUser = $env:POSTGRES_USER
if ([string]::IsNullOrEmpty($dbUser)) { $dbUser = "postgres" }
$dbPassword = $env:POSTGRES_PASSWORD
if ([string]::IsNullOrEmpty($dbPassword)) { $dbPassword = "postgres" }
$connectionString = "Host=postgres;Port=5432;Database=$dbName;Username=$dbUser;Password=$dbPassword"

docker run --rm `
  --network $networkName `
  -v "${PWD}:/src" `
  -w /src `
  mcr.microsoft.com/dotnet/sdk:10.0 `
  sh -c "dotnet tool install --global dotnet-ef --version 10.0.0 > /dev/null 2>&1 && export PATH=`$PATH:/root/.dotnet/tools && dotnet restore src/GymDogs.Presentation/GymDogs.Presentation.csproj > /dev/null 2>&1 && dotnet ef database update --project src/GymDogs.Infrastructure/GymDogs.Infrastructure.csproj --startup-project src/GymDogs.Presentation/GymDogs.Presentation.csproj --connection '$connectionString'"

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "  Setup completed successfully!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Services:" -ForegroundColor Cyan
    Write-Host "  - API: http://localhost:8080" -ForegroundColor White
    Write-Host "  - pgAdmin: http://localhost:5051" -ForegroundColor White
    Write-Host ""
    Write-Host "pgAdmin credentials:" -ForegroundColor Cyan
    $pgAdminEmail = $env:PGADMIN_EMAIL
    if ([string]::IsNullOrEmpty($pgAdminEmail)) { $pgAdminEmail = "admin@gymdogs.com" }
    $pgAdminPassword = $env:PGADMIN_PASSWORD
    if ([string]::IsNullOrEmpty($pgAdminPassword)) { $pgAdminPassword = "admin" }
    Write-Host "  Email: $pgAdminEmail" -ForegroundColor White
    Write-Host "  Password: $pgAdminPassword" -ForegroundColor White
    Write-Host ""
} else {
    Write-Host ""
    Write-Host "Migration failed!" -ForegroundColor Red
    exit $LASTEXITCODE
}
