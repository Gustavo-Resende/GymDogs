Write-Host "Running database migrations..." -ForegroundColor Green

$networkName = "gymdogs_gymdogs-network"
$connectionString = "Host=postgres;Port=5432;Database=GymDogsDb;Username=postgres;Password=postgres"

docker run --rm `
  --network $networkName `
  -v "${PWD}:/src" `
  -w /src `
  mcr.microsoft.com/dotnet/sdk:10.0 `
  sh -c "dotnet tool install --global dotnet-ef --version 10.0.0 > /dev/null 2>&1 && export PATH=`$PATH:/root/.dotnet/tools && dotnet restore src/GymDogs.Presentation/GymDogs.Presentation.csproj > /dev/null 2>&1 && dotnet ef database update --project src/GymDogs.Infrastructure/GymDogs.Infrastructure.csproj --startup-project src/GymDogs.Presentation/GymDogs.Presentation.csproj --connection '$connectionString'"

if ($LASTEXITCODE -eq 0) {
    Write-Host "Migrations completed successfully!" -ForegroundColor Green
} else {
    Write-Host "Migration failed!" -ForegroundColor Red
    exit $LASTEXITCODE
}
