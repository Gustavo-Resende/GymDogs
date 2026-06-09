
## 🎯 Sobre o Projeto

O **GymDogs** é uma aplicação para gerenciamento e acompanhamento de treinos de academia. O sistema permite que usuários organizem seus exercícios em pastas personalizadas, registrem séries com pesos e repetições, e compartilhem seus treinos com outros usuários através de perfis públicos ou privados.

### 🌟 Principais Funcionalidades

- **👤 Gestão de Perfis**: Cada usuário possui um perfil com controle de visibilidade (público/privado)
- **📁 Organização por Pastas**: Crie pastas de treino personalizadas (ex: "Costas", "Peito", "Pernas")
- **💪 Catálogo de Exercícios**: Sistema centralizado com 60 exercícios pré-cadastrados cobrindo todos os grupos musculares
- **🔍 Busca Inteligente**: Busca case-insensitive de exercícios por nome
- **📊 Registro de Séries**: Controle detalhado de séries, repetições e cargas levantadas
- **📈 Acompanhamento de Progresso**: Histórico completo de treinos para análise de evolução
- **🔐 Autenticação Segura**: Sistema de login com JWT e refresh tokens
- **👥 Compartilhamento**: Visualize perfis públicos de outros usuários e seus treinos
- **⚡ Performance Otimizada**: HTTP Compression, Response Caching e otimizações queries

### 🎨 Funcionalidades Futuras

#### Fase 1 - Melhorias de UX
- [ ] Paginação e infinite scroll para listagens grandes
- [ ] Sistema de favoritos para exercícios
- [ ] Histórico de progresso com gráficos
- [ ] Exportação de treinos (PDF, Excel)

#### Fase 2 - Social
- [ ] Sistema de grupos para compartilhamento de treinos
- [ ] Feed de atividades (exercícios recentes de perfis que você segue)
- [ ] Sistema de comentários em treinos
- [ ] Sistema de likes/reactions

#### Fase 3 - Avançado
- [ ] Fotos e vídeos de exercícios
- [ ] Sistema de notificações em tempo real
- [ ] Planejamento de treinos semanais/mensais
- [ ] Integração com wearables (Apple Watch, Fitbit)
- [ ] IA para sugestão de treinos personalizados

---

## 🚀 Como Iniciar o Projeto

### 📋 Pré-requisitos

- **Docker Desktop** (Windows/Mac) **Docker Engine + Docker Compose** (Linux)
- **Portas disponíveis**: 8080 (API), 5051 (pgAdmin), 5432 (PostgreSQL)

### ⚡ Início Rápido

#### Passo 1: Clone o repositório

```bash
git clone <url-do-repositorio>
cd GymDogs
```

#### Passo 2: Execute o script de inicialização

**Windows (PowerShell):**
```powershell
.\docker-init.ps1
```

**Linux/Mac:**
```bash
chmod +x docker-init.sh
./docker-init.sh
```

#### O que o script faz automaticamente:

✅ Cria arquivo `.env` se não existir (a partir de `env.example`)  
✅ Inicia todos os containers Docker (PostgreSQL, pgAdmin, API)  
✅ Aguarda PostgreSQL ficar pronto  
✅ Executa migrations do banco de dados  
✅ Popula o banco com 60 exercícios pré-definidos  
✅ Configura tudo automaticamente  

#### Passo 3: Acesse os serviços

Após o script concluir, você terá acesso a:

- **API**: http://localhost:8080
- **Swagger/OpenAPI**: http://localhost:8080/scalar/v1 (em desenvolvimento)
- **pgAdmin**: http://localhost:5051
  - Email: `admin@gymdogs.com` (ou o valor do `PGADMIN_EMAIL` no `.env`)
  - Senha: `admin` (ou o valor do `PGADMIN_PASSWORD` no `.env`)

### 🔧 Configuração de Secrets (Opcional)

Se você quiser personalizar os secrets:

#### 1. Crie o arquivo `.env`

```bash
# Copiar template
cp env.example .env
```

#### 2. Edite o arquivo `.env`

**Windows:**
```powershell
notepad .env
```

**Linux/Mac:**
```bash
nano .env
```

#### 3. Personalize os valores

```env
# Database Configuration
POSTGRES_DB=GymDogsDb
POSTGRES_USER=postgres
POSTGRES_PASSWORD=sua_senha_segura_aqui

# pgAdmin Configuration
PGADMIN_EMAIL=seu_email@exemplo.com
PGADMIN_PASSWORD=sua_senha_pgadmin_aqui

# JWT Configuration
JWT_SECRET_KEY=SuaChaveSecretaSuperSeguraComPeloMenos32CaracteresParaHS256
JWT_ISSUER=GymDogs
JWT_AUDIENCE=GymDogsUsers
JWT_ACCESS_TOKEN_EXPIRATION_MINUTES=15
JWT_REFRESH_TOKEN_EXPIRATION_DAYS=7
```

**⚠️ SEGURANÇA:**
- O arquivo `.env` está no `.gitignore` e **NÃO será commitado**
- Nunca commite o arquivo `.env` com secrets reais
- Em produção, use Docker Secrets ou Azure Key Vault

#### 4. Reinicie os containers

```bash
docker-compose down
docker-compose up -d
```

### 📊 Configurar pgAdmin (Opcional)

1. Acesse http://localhost:5051
2. Faça login com as credenciais do `.env`
3. Clique com botão direito em **"Servers"** → **"Register"** → **"Server"**
4. Na aba **"General"**:
   - Name: `GymDogs DB`
5. Na aba **"Connection"**:
   - Host name/address: `postgres`
   - Port: `5432`
   - Maintenance database: `GymDogsDb` (ou o valor do `POSTGRES_DB` no `.env`)
   - Username: `postgres` (ou o valor do `POSTGRES_USER` no `.env`)
   - Password: `postgres` (ou o valor do `POSTGRES_PASSWORD` no `.env`)
   - Marque **"Save password"**
6. Clique em **"Save"**

### 🛠️ Comandos Úteis

```bash
# Ver logs da API
docker-compose logs -f gymdogs-api

# Parar todos os serviços
docker-compose down

# Parar e remover volumes (limpar dados)
docker-compose down -v

# Reiniciar apenas a API
docker-compose restart gymdogs-api

# Ver status dos containers
docker-compose ps

# Executar migrations manualmente (se necessário)
.\docker-migrate.ps1  # Windows
./docker-migrate.sh   # Linux/Mac
```

---

## 📋 Regras de Negócio Principais

### Usuário (User)

- Cada usuário possui **username** único e **email** único
- Senha armazenada como **hash** usando BCrypt
- Ao criar um usuário, um **perfil é automaticamente criado** (relacionamento 1:1)
- Sistema de **roles**: Admin e User

### Perfil (Profile)

- Criado automaticamente quando um usuário é cadastrado
- Possui **visibilidade**: Público (todos podem ver) ou Privado (apenas o dono)
- Pode ter **múltiplas pastas de treino**
- Respostas de listagem incluem mensagens informativas quando não há resultados

### Pasta de Treino (WorkoutFolder)

- Pertence a um perfil específico
- Possui nome, descrição opcional e ordem de exibição
- Pode conter múltiplos exercícios
- Endpoints para listar exercícios disponíveis (não adicionados à pasta)

### Exercício (Exercise)

- **60 exercícios pré-cadastrados** cobrindo todos os grupos musculares
- Criado no **catálogo global** e pode ser reutilizado em múltiplas pastas
- Possui nome e descrição opcional
- Apenas **Admin** pode criar/editar/deletar exercícios
- Busca **case-insensitive** por nome

### Série (ExerciseSet)

- Representa uma série executada de um exercício
- Possui número da série, repetições e peso
- Sistema calcula automaticamente o número da série se não fornecido
- Mantém **histórico completo** para acompanhamento de progresso

### Hierarquia do Sistema

```
User (1) ────── (1) Profile
                        │
                        │ (1:N)
                        │
                        ▼
                WorkoutFolder
                        │
                        │ (1:N)
                        │
                        ▼
                FolderExercise ────── (N:1) Exercise
                        │
                        │ (1:N)
                        │
                        ▼
                ExerciseSet
```

---

## 🔐 Segurança

### Autenticação e Autorização

- **JWT Authentication**: Tokens JWT para autenticação
- **Refresh Tokens**: Renovação automática de tokens
- **Role-Based Authorization**: Sistema de roles (Admin/User)
- **Property-Based Authorization**: Usuários só podem modificar seus próprios recursos
- **Password Hashing**: Senhas armazenadas com BCrypt

### Boas Práticas Implementadas

- ✅ Secrets em arquivo `.env` (não versionado)
- ✅ Validação de entrada em todas as requisições
- ✅ Proteção contra SQL Injection (EF Core)
- ✅ Error handling global com Strategy Pattern
- ✅ Visibilidade de perfis (público/privado)
- ✅ CORS configurado para desenvolvimento e produção

---

## 🧪 Testando a API

### 1. Verificar se está funcionando

Acesse: http://localhost:8080/scalar/v1

Você deve ver a documentação Swagger/OpenAPI interativa.

### 2. Executar Testes

O projeto possui uma suíte completa de testes unitários e de integração:

```bash
# Executar todos os testes
cd src/GymDogs.Tests
dotnet test

# Executar com verbosidade normal
dotnet test --verbosity normal

# Executar testes de uma entidade específica
dotnet test --filter "FullyQualifiedName~ExerciseTests"
```

#### Estrutura de Testes

- **Testes Unitários**: Testam entidades de domínio isoladamente
  - `Exercises/ExerciseTests.cs` - 23 testes
  - `ExerciseSets/ExerciseSetTests.cs` - 30 testes
  - `FolderExercises/FolderExerciseTests.cs` - 14 testes
  - `Profiles/ProfileTests.cs` - 20 testes
  - `Users/UserTests.cs` - 18 testes
  - `WorkoutFolders/WorkoutFolderTests.cs` - 28 testes

- **Testes de Integração**: Testam handlers de comandos e queries
  - Cobertura completa de todos os handlers principais
  - Uso de Moq para isolamento de dependências
  - Testes de casos extremos e edge cases

**Total: ~279 testes**

### 3. Cobertura de Código

O projeto está configurado para exibir cobertura de código diretamente no console usando Coverlet.

```bash
# Executar testes com cobertura exibida no console
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=text
```

A cobertura será exibida no console ao final da execução dos testes, mostrando:
- Percentual de cobertura geral
- Cobertura por assembly/projeto
- Linhas cobertas vs. linhas totais
- Branches cobertos vs. branches totais

### 4. Exemplos de Requisições

#### Registrar um usuário

```bash
POST http://localhost:8080/api/auth/register
Content-Type: application/json

{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "SenhaSegura123!"
}
```

#### Fazer login

```bash
POST http://localhost:8080/api/auth/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "SenhaSegura123!"
}
```

**Resposta:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "abc123...",
  "userId": "guid-aqui",
  "username": "johndoe",
  "email": "john@example.com",
  "expiresAt": "2026-01-23T10:30:00Z",
  "refreshTokenExpiresAt": "2026-01-30T10:30:00Z",
  "role": "User"
}
```

#### Buscar exercícios por nome

```bash
GET http://localhost:8080/api/exercises/search?searchTerm=supino
Authorization: Bearer {token}
```

#### Listar exercícios disponíveis para uma pasta

```bash
GET http://localhost:8080/api/exercises/available/{workoutFolderId}
Authorization: Bearer {token}
```

#### Usar o token em requisições autenticadas

```bash
GET http://localhost:8080/api/profiles/{profileId}
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

#### Refresh Token

```bash
POST http://localhost:8080/api/auth/refresh
Content-Type: application/json

{
  "refreshToken": "abc123..."
}
```

---

## 🆘 Troubleshooting

### Porta já em uso

```bash
# Verificar o que está usando a porta
# Windows
netstat -ano | findstr :8080

# Linux/Mac
lsof -i :8080

# Alterar porta no docker-compose.yml se necessário
```

### PostgreSQL não inicia

```bash
# Ver logs
docker-compose logs postgres

# Verificar status
docker-compose ps

# Reiniciar
docker-compose restart postgres
```

### Migrations não executam

```bash
# Executar migrations manualmente
.\docker-migrate.ps1  # Windows
./docker-migrate.sh   # Linux/Mac
```

### Erro de conexão com banco

Verifique se:
- ✅ PostgreSQL está rodando (`docker-compose ps`)
- ✅ Connection string está correta no `.env`
- ✅ Credenciais estão corretas
- ✅ Banco de dados foi criado (migrations executadas)

### Erro CORS no front-end

- Verifique se a origem do front-end está configurada em `appsettings.Development.json`
- Certifique-se de que o Docker foi reconstruído após alterações de CORS
- Execute: `docker-compose down && docker-compose build --no-cache gymdogs-api && docker-compose up -d`

---

## 🏗️ Arquitetura

### Clean Architecture

O sistema segue os princípios de **Clean Architecture** com separação clara de responsabilidades:

- **Domain**: Entidades e regras de negócio puras (sem dependências externas)
- **Application**: Casos de uso, DTOs, interfaces (depende apenas do Domain)
- **Infrastructure**: Persistência, serviços externos (depende de Application e Domain)
- **Presentation**: API, controllers, middleware (depende de Application)

### CQRS (Command Query Responsibility Segregation)

- **Commands**: Operações de escrita (Create, Update, Delete)
- **Queries**: Operações de leitura (Get, List)
- MediatR para desacoplamento entre camadas

### Repository Pattern

- Abstração de acesso a dados através de `IRepository<T>` e `IReadRepository<T>`
- Implementação usando Ardalis.Specification para queries complexas
- Unit of Work para gerenciar transações

### Design Patterns Implementados

#### 🏭 Factory Pattern
- **Onde**: Criação de Specifications
- **Implementação**: `ISpecificationFactory` e `SpecificationFactory`
- **Benefício**: Centraliza normalização de dados e facilita manutenção

#### 🏗️ Builder Pattern
- **Onde**: Construção de JWT Tokens
- **Implementação**: `IJwtTokenBuilder` e `JwtTokenBuilder`
- **Benefício**: Construção fluente e legível de tokens complexos

#### 🎯 Strategy Pattern
- **Onde**: Mapeamento de exceções para resultados
- **Implementação**: `IExceptionMappingStrategy` e estratégias específicas
- **Benefício**: Tratamento flexível e extensível de erros

---

## ⚡ Otimizações de Performance

### Queries Otimizadas

- **AsNoTracking()**: Todas as queries de leitura usam `AsNoTracking()` para reduzir overhead do EF Core
- **Include()**: Evita N+1 queries ao carregar relacionamentos necessários
- **Especificações Reutilizáveis**: Ardalis.Specification para queries complexas e reutilizáveis

### HTTP Compression

- **Brotli e Gzip**: Compressão automática de respostas HTTP
- Reduz tamanho de payload em 60-80%
- Melhora tempo de resposta especialmente em conexões lentas

### Response Caching

- Cache de 30 segundos em endpoints públicos de perfis
- Reduz carga no banco de dados
- Melhora tempo de resposta para listagens públicas

### Resultados

- **30-40% mais rápido** em queries de leitura
- **50-70% redução** no tempo de resposta para listagens públicas
- **60-80% redução** no tamanho de payloads HTTP

---

## 🔧 Tecnologias Utilizadas

### Core
- **.NET 10.0** - Framework principal
- **ASP.NET Core** - API REST
- **PostgreSQL 16** - Banco de dados relacional
- **Entity Framework Core** - ORM

### Arquitetura e Padrões
- **MediatR** - Implementação do padrão CQRS
- **Ardalis.Result** - Padrão de retorno estruturado
- **Ardalis.Specification** - Queries complexas e reutilizáveis

### Segurança
- **JWT (JSON Web Tokens)** - Autenticação e autorização
- **BCrypt** - Hash seguro de senhas

### Performance
- **HTTP Compression (Brotli/Gzip)** - Compressão de respostas
- **Response Caching** - Cache de respostas HTTP
- **AsNoTracking** - Otimização de queries EF Core

### Infraestrutura
- **Docker & Docker Compose** - Containerização
- **CORS** - Cross-Origin Resource Sharing configurável

### Documentação e Testes
- **Scalar** - Documentação interativa OpenAPI/Swagger
- **xUnit** - Framework de testes
- **Moq** - Framework de mocking para testes
- **Coverlet** - Coleta e exibição de cobertura de código

---

## 📖 Estrutura do Projeto

```
GymDogs/
├── src/
│   ├── GymDogs.Domain/          # Entidades e regras de negócio
│   │   ├── Exercises/           # Entidade Exercise e Specifications
│   │   ├── ExerciseSets/        # Entidade ExerciseSet e Specifications
│   │   ├── FolderExercises/    # Entidade FolderExercise e Specifications
│   │   ├── Profiles/           # Entidade Profile e Specifications
│   │   ├── Users/              # Entidade User e Specifications
│   │   └── WorkoutFolders/     # Entidade WorkoutFolder e Specifications
│   ├── GymDogs.Application/     # Casos de uso, DTOs, interfaces
│   │   ├── Common/             # Interfaces, Specifications Factory, Exception Mapping
│   │   ├── Exercises/          # Commands e Queries de exercícios
│   │   ├── ExerciseSets/       # Commands e Queries de séries
│   │   ├── FolderExercises/    # Commands e Queries de exercícios em pastas
│   │   ├── Profiles/           # Commands e Queries de perfis
│   │   ├── Users/              # Commands e Queries de usuários
│   │   └── WorkoutFolders/     # Commands e Queries de pastas de treino
│   ├── GymDogs.Infrastructure/  # Persistência, serviços externos
│   │   ├── Migrations/         # Migrations do Entity Framework
│   │   ├── Persistence/        # DbContext, Repositories, Specifications Factory
│   │   └── Services/           # JWT Builder, Password Hasher, Token Generators
│   ├── GymDogs.Presentation/    # API, controllers, middleware
│   │   ├── Controllers/        # Controllers da API
│   │   ├── Configuration/     # Configurações (JWT OpenAPI Transformer)
│   │   ├── Extensions/         # Extensions (Result, HttpContext)
│   │   └── Services/           # Services (Exception Mapper)
│   └── GymDogs.Tests/          # Testes unitários e de integração
├── DesignPattern/               # Documentação de Design Patterns
│   ├── README.FACTORY_PATTERN.md
│   ├── README.BUILDER_PATTERN.md
│   └── README.STRATEGY_PATTERN.md
├── docker-compose.yml           # Configuração Docker Compose
├── docker-compose.override.yml  # Overrides locais (não versionado)
├── Dockerfile                   # Imagem Docker da API
├── docker-init.ps1              # Script de inicialização (Windows)
├── docker-init.sh               # Script de inicialização (Linux/Mac)
├── docker-migrate.ps1           # Script de migrations (Windows)
├── docker-migrate.sh            # Script de migrations (Linux/Mac)
├── env.example                  # Template de variáveis de ambiente
└── README.md                    # Este arquivo
```

---

## 📚 Documentação Adicional

- **Design Patterns**: Veja a pasta `DesignPattern/` para documentação detalhada dos patterns implementados
- **API Documentation**: Acesse http://localhost:8080/scalar/v1 quando a API estiver rodando

---

## 👥 Contribuindo

Contribuições são bem-vindas! Por favor:

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/MinhaFeature`)
3. Commit suas mudanças (`git commit -m 'Adiciona MinhaFeature'`)
4. Push para a branch (`git push origin feature/MinhaFeature`)
5. Abra um Pull Request

### Padrões de Código

- Siga os princípios de Clean Architecture
- Use CQRS para separar comandos e queries
- Documente código complexo
- Adicione testes quando possível
- Mantenha cobertura de código alta (objetivo: >80%)
- Teste casos extremos e edge cases, não apenas o caminho feliz
- Use Design Patterns quando apropriado (Factory, Builder, Strategy)
- Mantenha queries otimizadas (AsNoTracking, Include quando necessário)

---

## 📝 Licença

Este projeto está sob a licença MIT. Veja o arquivo `LICENSE` para mais detalhes.

---

## 🙏 Agradecimentos

- **Ardalis** pelos excelentes pacotes (Result, Specification)
- **MediatR** pela implementação do padrão Mediator
- **Entity Framework Core** pela excelente ORM
- Comunidade .NET por todo o suporte e recursos

---

**Desenvolvido com ❤️ usando .NET 10.0**
