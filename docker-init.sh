#!/bin/bash

echo "========================================"
echo "  GymDogs - Docker Setup & Migration"
echo "========================================"
echo ""

# Check if .env exists
if [ ! -f .env ]; then
    echo "Creating .env file from env.example..."
    if [ -f env.example ]; then
        cp env.example .env
        echo ".env file created. Please review and update secrets if needed!"
    else
        echo "Warning: env.example not found. Creating .env with default values..."
        cat > .env << EOF
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
EOF
    fi
    echo ""
fi

# Load .env file
export $(cat .env | grep -v '^#' | xargs)

# Start containers
echo "Starting Docker containers..."
docker-compose up -d

if [ $? -ne 0 ]; then
    echo "Failed to start containers!"
    exit 1
fi

echo "Waiting for PostgreSQL to be ready..."
sleep 5

# Wait for PostgreSQL to be healthy
max_attempts=30
attempt=0
while [ $attempt -lt $max_attempts ]; do
    if docker-compose exec -T postgres pg_isready -U postgres > /dev/null 2>&1; then
        echo "PostgreSQL is ready!"
        break
    fi
    attempt=$((attempt + 1))
    echo "Waiting for PostgreSQL... ($attempt/$max_attempts)"
    sleep 2
done

if [ $attempt -eq $max_attempts ]; then
    echo "PostgreSQL failed to become ready!"
    exit 1
fi

# Run migrations
echo ""
echo "Running database migrations..."

NETWORK_NAME="gymdogs_gymdogs-network"
DB_NAME="${POSTGRES_DB:-GymDogsDb}"
DB_USER="${POSTGRES_USER:-postgres}"
DB_PASSWORD="${POSTGRES_PASSWORD:-postgres}"
CONNECTION_STRING="Host=postgres;Port=5432;Database=$DB_NAME;Username=$DB_USER;Password=$DB_PASSWORD"

docker run --rm \
  --network $NETWORK_NAME \
  -v "$(pwd):/src" \
  -w /src \
  mcr.microsoft.com/dotnet/sdk:10.0 \
  sh -c "dotnet tool install --global dotnet-ef --version 10.0.0 > /dev/null 2>&1 && export PATH=\$PATH:/root/.dotnet/tools && dotnet restore src/GymDogs.Presentation/GymDogs.Presentation.csproj > /dev/null 2>&1 && dotnet ef database update --project src/GymDogs.Infrastructure/GymDogs.Infrastructure.csproj --startup-project src/GymDogs.Presentation/GymDogs.Presentation.csproj --connection '$CONNECTION_STRING'"

if [ $? -eq 0 ]; then
    echo ""
    echo "========================================"
    echo "  Setup completed successfully!"
    echo "========================================"
    echo ""
    echo "Services:"
    echo "  - API: http://localhost:8080"
    echo "  - pgAdmin: http://localhost:5051"
    echo ""
    echo "pgAdmin credentials:"
    echo "  Email: ${PGADMIN_EMAIL:-admin@gymdogs.com}"
    echo "  Password: ${PGADMIN_PASSWORD:-admin}"
    echo ""
else
    echo ""
    echo "Migration failed!"
    exit 1
fi
