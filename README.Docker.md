# Docker Setup - GymDogs

## 🐳 Containerização

Este projeto está configurado para rodar em containers Docker usando Docker Compose.

## 📋 Pré-requisitos

- Docker Desktop (Windows/Mac) ou Docker Engine + Docker Compose (Linux)
- Portas disponíveis: 8080 (API), 5051 (pgAdmin), 5432 (PostgreSQL)

## 🚀 Como usar

### Setup Inicial (Recomendado)

**Windows (PowerShell):**
```powershell
.\docker-init.ps1
```

**Linux/Mac:**
```bash
chmod +x docker-init.sh
./docker-init.sh
```

Este script automaticamente:
- Cria arquivo `.env` se não existir (a partir de `env.example`)
- Inicia todos os containers
- Aguarda PostgreSQL ficar pronto
- Executa migrations do banco de dados
- Configura tudo automaticamente

### Iniciar todos os serviços manualmente

```bash
docker-compose up -d
```

### Ver logs

```bash
docker-compose logs -f gymdogs-api
```

### Parar todos os serviços

```bash
docker-compose down
```

### Parar e remover volumes (limpar dados)

```bash
docker-compose down -v
```

### Rebuild da aplicação

```bash
docker-compose build gymdogs-api
docker-compose up -d gymdogs-api
```

## 🔧 Serviços

### 1. PostgreSQL (postgres)
- **Porta**: 5432
- **Database**: GymDogsDb
- **Usuário**: postgres
- **Senha**: postgres
- **Volume**: Dados persistidos em `postgres_data`

### 2. pgAdmin (pgadmin)
- **URL**: http://localhost:5051
- **Email**: admin@gymdogs.com
- **Senha**: admin
- **Volume**: Configurações persistidas em `pgadmin_data`

#### Configurar servidor no pgAdmin:
1. Acesse http://localhost:5051
2. Faça login com as credenciais acima
3. Clique com botão direito em "Servers" → "Register" → "Server"
4. Na aba "General":
   - Name: GymDogs DB
5. Na aba "Connection":
   - Host name/address: `postgres`
   - Port: `5432`
   - Maintenance database: `GymDogsDb`
   - Username: `postgres`
   - Password: `postgres`
   - Marque "Save password"
6. Clique em "Save"

### 3. GymDogs API (gymdogs-api)
- **URL**: http://localhost:8080
- **Swagger/OpenAPI**: http://localhost:8080/openapi/v1.json
- **Scalar UI**: http://localhost:8080/scalar/v1 (em desenvolvimento)

## 🗄️ Executar Migrations

Após iniciar os containers, execute as migrations usando o script:

**Windows (PowerShell):**
```powershell
.\docker-migrate.ps1
```

**Linux/Mac:**
```bash
chmod +x docker-migrate.sh
./docker-migrate.sh
```

O script usa um container temporário com .NET SDK para executar as migrations, garantindo que todas as dependências estejam disponíveis.

**Alternativa - Executar localmente (se tiver .NET SDK instalado):**
```bash
dotnet ef database update --project src/GymDogs.Infrastructure/GymDogs.Infrastructure.csproj --startup-project src/GymDogs.Presentation/GymDogs.Presentation.csproj --connection "Host=localhost;Database=GymDogsDb;Username=postgres;Password=postgres"
```

## 🔐 Variáveis de Ambiente e Secrets

### Arquivo .env

**IMPORTANTE:** Crie um arquivo `.env` na raiz do projeto com seus secrets:

```bash
# Copiar template
cp env.example .env

# Editar com seus secrets reais
# Windows: notepad .env
# Linux/Mac: nano .env
```

**Variáveis disponíveis:**
- `POSTGRES_DB`: Nome do banco de dados
- `POSTGRES_USER`: Usuário do PostgreSQL
- `POSTGRES_PASSWORD`: **Senha do PostgreSQL (SECRET)**
- `PGADMIN_EMAIL`: Email do pgAdmin
- `PGADMIN_PASSWORD`: **Senha do pgAdmin (SECRET)**
- `JWT_SECRET_KEY`: **Chave secreta JWT (SECRET - altere em produção!)**
- `JWT_ISSUER`: Emissor do JWT
- `JWT_AUDIENCE`: Audience do JWT
- `JWT_ACCESS_TOKEN_EXPIRATION_MINUTES`: Expiração do access token
- `JWT_REFRESH_TOKEN_EXPIRATION_DAYS`: Expiração do refresh token

**⚠️ SEGURANÇA:**
- O arquivo `.env` está no `.gitignore` e **NÃO será commitado**
- Nunca commite o arquivo `.env` com secrets reais
- Use `env.example` como template
- Em produção, use Docker Secrets ou Azure Key Vault

## 🛠️ Comandos Úteis

### Ver status dos containers
```bash
docker-compose ps
```

### Acessar shell do container da API
```bash
docker-compose exec gymdogs-api sh
```

### Acessar PostgreSQL via CLI
```bash
docker-compose exec postgres psql -U postgres -d GymDogsDb
```

### Ver logs de todos os serviços
```bash
docker-compose logs -f
```

## 📝 Notas

- Os dados do PostgreSQL e pgAdmin são persistidos em volumes Docker
- A API aguarda o PostgreSQL estar saudável antes de iniciar
- Em produção, altere as senhas padrão e use secrets do Docker
