# 🌱 Database Seeding - Exercícios Pré-cadastrados

Este documento explica como funciona o sistema de seeding (dados iniciais) do banco de dados, especificamente para exercícios pré-cadastrados.

---

## 📋 Índice

1. [O que é Seeding?](#o-que-é-seeding)
2. [Por que foi implementado?](#por-que-foi-implementado)
3. [Como funciona?](#como-funciona)
4. [Exercícios Cadastrados](#exercícios-cadastrados)
5. [Como aplicar o seeding?](#como-aplicar-o-seeding)
6. [Estrutura Técnica](#estrutura-técnica)
7. [Manutenção e Expansão](#manutenção-e-expansão)

---

## 🌱 O que é Seeding?

**Seeding** (ou "semeadura" em português) é o processo de popular o banco de dados com dados iniciais quando ele é criado pela primeira vez. É como plantar sementes que vão crescer e se tornar a base do sistema.

### Analogia do Mundo Real

Imagine que você está abrindo uma biblioteca. Antes de abrir as portas, você precisa ter alguns livros nas prateleiras para que os clientes possam começar a usar o serviço. O seeding é exatamente isso: colocar os "livros básicos" (exercícios) na "biblioteca" (banco de dados) antes dos usuários começarem a usar.

---

## ❓ Por que foi implementado?

### Problema Identificado

Quando um novo usuário criava uma conta e tentava adicionar exercícios aos seus treinos, a tela de exercícios estava vazia porque não havia nenhum exercício cadastrado no banco de dados. Isso criava uma experiência ruim:

- ❌ Usuário não conseguia começar a usar o sistema imediatamente
- ❌ Precisava cadastrar manualmente cada exercício (trabalhoso)
- ❌ Exercícios básicos como "Supino Reto" e "Rosca Direta" não estavam disponíveis

### Solução

Implementamos um sistema de seeding que **automaticamente** cadastra **60 exercícios básicos** organizados por grupo muscular quando o banco de dados é criado ou quando as migrations são executadas.

**Benefícios:**
- ✅ Usuários podem começar a usar o sistema imediatamente
- ✅ Exercícios básicos já estão disponíveis
- ✅ Não precisa cadastrar manualmente exercícios comuns
- ✅ Base sólida para começar a treinar

---

## ⚙️ Como funciona?

### Processo Automático

O seeding acontece **automaticamente** quando você executa as migrations do Entity Framework Core:

1. **Migration criada:** `20260124210246_SeedExercises.cs`
2. **Execução:** Quando você roda `dotnet ef database update` ou usa o script `docker-migrate.ps1`
3. **Resultado:** 60 exercícios são inseridos no banco de dados

### Quando o Seeding é Aplicado?

- ✅ **Primeira vez:** Quando você cria o banco de dados pela primeira vez
- ✅ **Após limpar o banco:** Se você deletar o banco e recriar (`docker-compose down -v`)
- ✅ **Nunca duplica:** O EF Core verifica se os exercícios já existem antes de inserir (usando IDs fixos)

### Proteção contra Duplicação

Os exercícios têm **IDs fixos** (GUIDs predefinidos), então:
- Se o exercício já existe (mesmo ID), ele **não é inserido novamente**
- Se o exercício não existe, ele **é inserido**
- Isso garante que o seeding pode ser executado múltiplas vezes sem criar duplicatas

---

## 💪 Exercícios Cadastrados

### Total: 60 Exercícios

Os exercícios estão organizados por grupo muscular:

#### 🏋️ Peitoral (6 exercícios)
- Supino Reto
- Supino Inclinado
- Supino Declinado
- Crucifixo
- Flexão de Braço
- Peck Deck

#### 🎯 Costas (7 exercícios)
- Barra Fixa
- Remada Curvada
- Puxada Frontal
- Puxada Atrás
- Remada Unilateral
- Remada no TRX
- Serrote

#### 💪 Ombros (6 exercícios)
- Desenvolvimento com Halteres
- Elevação Lateral
- Elevação Frontal
- Elevação Posterior
- Desenvolvimento Arnold
- Crucifixo Invertido

#### 💪 Bíceps (6 exercícios)
- Rosca Direta
- Rosca Alternada
- Rosca Martelo
- Rosca Concentrada
- Rosca Scott
- Rosca 21

#### 💪 Tríceps (6 exercícios)
- Tríceps Pulley
- Tríceps Testa
- Tríceps Coice
- Paralelas
- Tríceps Francês
- Mergulho

#### 🦵 Pernas - Quadríceps (7 exercícios)
- Agachamento Livre
- Agachamento com Barra
- Leg Press
- Extensão de Pernas
- Afundo
- Agachamento Búlgaro
- Hack Squat

#### 🦵 Pernas - Posterior (5 exercícios)
- Mesa Flexora
- Stiff
- Levantamento Terra
- Cadeira Flexora
- Good Morning

#### 🍑 Glúteos (5 exercícios)
- Elevação Pélvica
- Abdução de Quadril
- Avanço
- Agachamento Sumô
- Kickback

#### 🦵 Panturrilhas (4 exercícios)
- Panturrilha em Pé
- Panturrilha Sentado
- Panturrilha no Leg Press
- Panturrilha Unilateral

#### 🔥 Abdômen (8 exercícios)
- Abdominal Reto
- Prancha
- Abdominal Infra
- Abdominal Oblíquo
- Mountain Climber
- Abdominal Bicicleta
- Russian Twist
- Dead Bug

#### 💪 Antebraços (3 exercícios)
- Rosca de Punho
- Rosca de Punho Inversa
- Farmer's Walk

---

## 🚀 Como Aplicar o Seeding?

### Opção 1: Script Automático (Recomendado)

O seeding é aplicado **automaticamente** quando você usa o script de inicialização:

**Windows:**
```powershell
.\docker-init.ps1
```

**Linux/Mac:**
```bash
./docker-init.sh
```

O script executa as migrations automaticamente, incluindo o seeding.

### Opção 2: Migration Manual

Se você já tem o banco criado e quer aplicar apenas o seeding:

**Windows:**
```powershell
.\docker-migrate.ps1
```

**Linux/Mac:**
```bash
./docker-migrate.sh
```

### Opção 3: Comando Direto

```bash
cd src/GymDogs.Infrastructure
dotnet ef database update --startup-project ../GymDogs.Presentation/GymDogs.Presentation.csproj
```

### Verificar se Funcionou

Após executar as migrations, você pode verificar se os exercícios foram inseridos:

**Via API:**
```bash
GET http://localhost:8080/api/exercises
```

**Via pgAdmin:**
```sql
SELECT COUNT(*) FROM "Exercises";
-- Deve retornar 60
```

**Via SQL direto:**
```sql
SELECT "Name", "Description" FROM "Exercises" ORDER BY "Name";
```

---

## 🔧 Estrutura Técnica

### Arquivos Envolvidos

1. **`src/GymDogs.Infrastructure/Persistence/AppDbContext.cs`**
   - Método `SeedExercises()` que define os exercícios
   - Chamado no `OnModelCreating()`

2. **`src/GymDogs.Infrastructure/Migrations/20260124210246_SeedExercises.cs`**
   - Migration gerada pelo EF Core
   - Contém os comandos SQL para inserir os exercícios

3. **`src/GymDogs.Infrastructure/Migrations/20260124210246_SeedExercises.Designer.cs`**
   - Metadata da migration (gerado automaticamente)

### Como o Seeding é Implementado

```csharp
// AppDbContext.cs
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    
    // Seed de Exercícios Básicos
    SeedExercises(modelBuilder);
}

private void SeedExercises(ModelBuilder modelBuilder)
{
    var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    modelBuilder.Entity<Exercise>().HasData(
        new { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), 
              Name = "Supino Reto", 
              Description = "Exercício fundamental...", 
              CreatedAt = seedDate, 
              LastUpdatedAt = seedDate },
        // ... mais exercícios
    );
}
```

### IDs Fixos (GUIDs)

Cada exercício tem um **ID fixo** (GUID predefinido) para garantir:
- ✅ Não duplicar exercícios ao executar migrations múltiplas vezes
- ✅ Facilita referências em testes
- ✅ Permite atualizações futuras dos exercícios

**Padrão de IDs:**
- Peitoral: `11111111-1111-1111-1111-111111111111` até `11111111-1111-1111-1111-111111111116`
- Costas: `22222222-2222-2222-2222-222222222221` até `22222222-2222-2222-2222-222222222227`
- Ombros: `33333333-3333-3333-3333-333333333331` até `33333333-3333-3333-3333-333333333336`
- E assim por diante...

---

## 🔄 Manutenção e Expansão

### Como Adicionar Mais Exercícios

Se você quiser adicionar mais exercícios ao seeding:

1. **Edite `AppDbContext.cs`:**
   ```csharp
   private void SeedExercises(ModelBuilder modelBuilder)
   {
       // ... exercícios existentes ...
       
       // Adicione novos exercícios aqui
       new { Id = Guid.Parse("CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC"), 
             Name = "Novo Exercício", 
             Description = "Descrição...", 
             CreatedAt = seedDate, 
             LastUpdatedAt = seedDate }
   }
   ```

2. **Crie uma nova migration:**
   ```bash
   dotnet ef migrations add AddMoreExercises --startup-project ../GymDogs.Presentation/GymDogs.Presentation.csproj
   ```

3. **Execute a migration:**
   ```bash
   dotnet ef database update --startup-project ../GymDogs.Presentation/GymDogs.Presentation.csproj
   ```

### Como Atualizar Exercícios Existentes

Para atualizar um exercício existente (ex: corrigir descrição):

1. **Edite o exercício no `SeedExercises()`**
2. **Crie uma nova migration:**
   ```bash
   dotnet ef migrations add UpdateExerciseDescription --startup-project ../GymDogs.Presentation/GymDogs.Presentation.csproj
   ```
3. **A migration gerada terá comandos UPDATE** para atualizar os exercícios existentes

### Como Remover Exercícios do Seeding

**⚠️ ATENÇÃO:** Não remova exercícios que já estão sendo usados por usuários!

Se precisar remover:
1. Remova do método `SeedExercises()`
2. Crie migration para deletar (ou deixe no banco se já estiver em uso)

---

## 📊 Estrutura da Migration

A migration gerada pelo EF Core contém algo como:

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.InsertData(
        table: "Exercises",
        columns: new[] { "Id", "Name", "Description", "CreatedAt", "LastUpdatedAt" },
        values: new object[,]
        {
            { new Guid("11111111-1111-1111-1111-111111111111"), "Supino Reto", "Exercício fundamental...", new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            // ... mais exercícios
        });
}
```

O EF Core usa `INSERT` com verificação de existência para evitar duplicatas.

---

## ✅ Verificação

### Como Verificar se o Seeding Funcionou

**1. Via API:**
```bash
curl http://localhost:8080/api/exercises
```

**2. Via SQL:**
```sql
-- Contar exercícios
SELECT COUNT(*) FROM "Exercises";

-- Listar todos
SELECT "Name", "Description" FROM "Exercises" ORDER BY "Name";

-- Verificar por grupo (exemplo: peitoral)
SELECT "Name" FROM "Exercises" 
WHERE "Name" LIKE '%Supino%' OR "Name" LIKE '%Peitoral%';
```

**3. Via Front-end:**
- Acesse a tela de exercícios
- Você deve ver 60 exercícios disponíveis
- Pode buscar por nome (ex: "Supino", "Rosca", "Agachamento")

---

## 🎯 Benefícios para o Usuário

### Antes do Seeding
- ❌ Tela de exercícios vazia
- ❌ Usuário precisa cadastrar manualmente cada exercício
- ❌ Experiência ruim para novos usuários

### Depois do Seeding
- ✅ 60 exercícios básicos disponíveis imediatamente
- ✅ Usuário pode começar a treinar sem cadastrar exercícios
- ✅ Exercícios organizados por grupo muscular
- ✅ Base sólida para começar

---

## 📝 Notas Importantes

### ⚠️ IDs Fixos

- Os IDs dos exercícios são **fixos** (GUIDs predefinidos)
- Não altere os IDs de exercícios existentes
- Use novos IDs para novos exercícios

### ⚠️ Não Deletar Exercícios em Uso

- Se um exercício já está sendo usado em treinos de usuários, **não o remova**
- Remover pode quebrar referências existentes
- Considere marcar como "inativo" em vez de deletar

### ⚠️ Ordem de Execução

- O seeding é executado **após** a criação das tabelas
- Se você limpar o banco (`docker-compose down -v`), o seeding será aplicado novamente na próxima migration

---

## 🔍 Troubleshooting

### Problema: Exercícios não aparecem após migration

**Solução:**
1. Verifique se a migration foi executada:
   ```bash
   dotnet ef migrations list --startup-project ../GymDogs.Presentation/GymDogs.Presentation.csproj
   ```
2. Verifique se há erros nos logs do Docker
3. Execute a migration manualmente se necessário

### Problema: Exercícios duplicados

**Causa:** IDs diferentes para o mesmo exercício

**Solução:**
- Verifique se não há IDs duplicados no método `SeedExercises()`
- O EF Core não permite IDs duplicados na mesma migration

### Problema: Migration não cria os exercícios

**Solução:**
1. Verifique se o método `SeedExercises()` está sendo chamado no `OnModelCreating()`
2. Verifique se não há erros de compilação
3. Rebuild o projeto: `dotnet build`

---

## 📚 Referências

- [Entity Framework Core - Data Seeding](https://learn.microsoft.com/en-us/ef/core/modeling/data-seeding)
- [Migrations Overview](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)

---

**Última atualização:** Janeiro 2024  
**Total de exercícios:** 60  
**Migration:** `20260124210246_SeedExercises`
