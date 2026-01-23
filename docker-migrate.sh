#!/bin/bash

echo "Running database migrations..."

NETWORK_NAME="gymdogs_gymdogs-network"
CONNECTION_STRING="Host=postgres;Database=GymDogsDb;Username=postgres;Password=postgres"

docker run --rm \
  --network $NETWORK_NAME \
  -v "$(pwd):/src" \
  -w /src \
  mcr.microsoft.com/dotnet/sdk:10.0 \
  dotnet ef database update \
  --project src/GymDogs.Infrastructure/GymDogs.Infrastructure.csproj \
  --startup-project src/GymDogs.Presentation/GymDogs.Presentation.csproj \
  --connection "$CONNECTION_STRING"

echo "Migrations completed!"
