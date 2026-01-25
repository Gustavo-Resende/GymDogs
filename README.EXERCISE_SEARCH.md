# 🔍 Busca de Exercícios - Case-Insensitive e Exercícios Disponíveis

Este documento explica as funcionalidades de busca de exercícios implementadas no sistema, incluindo busca case-insensitive e filtro de exercícios disponíveis para pastas de treino.

---

## 📋 Índice

1. [Busca Case-Insensitive](#busca-case-insensitive)
2. [Exercícios Disponíveis para Pastas](#exercícios-disponíveis-para-pastas)
3. [Endpoints da API](#endpoints-da-api)
4. [Exemplos de Uso](#exemplos-de-uso)
5. [Implementação Técnica](#implementação-técnica)
6. [Troubleshooting](#troubleshooting)

---

## 🔤 Busca Case-Insensitive

### O que foi implementado?

A busca de exercícios agora é **case-insensitive** (não diferencia maiúsculas de minúsculas), permitindo que o usuário encontre exercícios independentemente de como digita o termo de busca.

### Como funciona?

**Antes:**
- Buscar "supino" (minúsculo) → ❌ Não encontrava "Supino Reto"
- Buscar "Supino" (maiúsculo) → ✅ Encontrava "Supino Reto"
- Buscar "SUPINO" (tudo maiúsculo) → ❌ Não encontrava "Supino Reto"

**Agora:**
- Buscar "supino" (minúsculo) → ✅ Encontra "Supino Reto", "Supino Inclinado", etc.
- Buscar "Supino" (maiúsculo) → ✅ Encontra "Supino Reto", "Supino Inclinado", etc.
- Buscar "SUPINO" (tudo maiúsculo) → ✅ Encontra "Supino Reto", "Supino Inclinado", etc.
- Buscar "SuPiNo" (misturado) → ✅ Encontra "Supino Reto", "Supino Inclinado", etc.

### Implementação

A busca usa `ToLower()` em ambos os lados da comparação, que é traduzido pelo EF Core para uma query case-insensitive no PostgreSQL:

```csharp
var lowerSearchTerm = searchTerm.ToLowerInvariant();
Query.Where(e => e.Name.ToLower().Contains(lowerSearchTerm))
```

---

## 📁 Exercícios Disponíveis para Pastas

### O que foi implementado?

Foram criados endpoints que retornam apenas os exercícios que **ainda não foram adicionados** a uma pasta de treino específica. Isso evita que o usuário tente adicionar o mesmo exercício duas vezes na mesma pasta.

### Caso de Uso

**Cenário:**
1. Usuário cria uma pasta de treino chamada "Peitoral"
2. Adiciona "Supino Reto" à pasta
3. Ao clicar em "Adicionar Exercício", o sistema mostra apenas exercícios que **não estão** na pasta
4. "Supino Reto" **não aparece** na lista de exercícios disponíveis
5. "Supino Inclinado", "Crucifixo", etc. **aparecem** normalmente

### Benefícios

- ✅ Evita duplicação de exercícios na mesma pasta
- ✅ Melhora a experiência do usuário
- ✅ Interface mais limpa e intuitiva
- ✅ Busca também funciona com filtro de nome (case-insensitive)

---

## 🌐 Endpoints da API

### 1. Buscar Exercícios por Nome (Case-Insensitive)

**Endpoint:** `GET /api/exercises/search`

**Query Parameters:**
- `searchTerm` (string, obrigatório): Termo de busca

**Resposta:**
- `200 OK`: Lista de exercícios encontrados
- `400 Bad Request`: Termo de busca inválido ou vazio

**Exemplo:**
```http
GET /api/exercises/search?searchTerm=supino
Authorization: Bearer {token}
```

**Resposta:**
```json
[
  {
    "id": "11111111-1111-1111-1111-111111111111",
    "name": "Supino Reto",
    "description": "Exercício fundamental para desenvolvimento do peitoral maior"
  },
  {
    "id": "11111111-1111-1111-1111-111111111112",
    "name": "Supino Inclinado",
    "description": "Trabalha principalmente a porção superior do peitoral"
  },
  {
    "id": "11111111-1111-1111-1111-111111111113",
    "name": "Supino Declinado",
    "description": "Foca na porção inferior do peitoral"
  }
]
```

---

### 2. Listar Exercícios Disponíveis para uma Pasta

**Endpoint:** `GET /api/exercises/available/{workoutFolderId}`

**Path Parameters:**
- `workoutFolderId` (Guid, obrigatório): ID da pasta de treino

**Resposta:**
- `200 OK`: Lista de exercícios disponíveis (não adicionados à pasta)
- `400 Bad Request`: ID da pasta inválido

**Exemplo:**
```http
GET /api/exercises/available/550e8400-e29b-41d4-a716-446655440000
Authorization: Bearer {token}
```

**Resposta:**
```json
[
  {
    "id": "11111111-1111-1111-1111-111111111112",
    "name": "Supino Inclinado",
    "description": "Trabalha principalmente a porção superior do peitoral"
  },
  {
    "id": "11111111-1111-1111-1111-111111111113",
    "name": "Supino Declinado",
    "description": "Foca na porção inferior do peitoral"
  },
  {
    "id": "44444444-4444-4444-4444-444444444441",
    "name": "Rosca Direta",
    "description": "Exercício fundamental para bíceps"
  }
]
```

**Nota:** Se "Supino Reto" já estiver na pasta, ele **não aparecerá** nesta lista.

---

### 3. Buscar Exercícios Disponíveis por Nome (Case-Insensitive)

**Endpoint:** `GET /api/exercises/available/{workoutFolderId}/search`

**Path Parameters:**
- `workoutFolderId` (Guid, obrigatório): ID da pasta de treino

**Query Parameters:**
- `searchTerm` (string, obrigatório): Termo de busca

**Resposta:**
- `200 OK`: Lista de exercícios disponíveis encontrados
- `400 Bad Request`: ID da pasta ou termo de busca inválido

**Exemplo:**
```http
GET /api/exercises/available/550e8400-e29b-41d4-a716-446655440000/search?searchTerm=rosca
Authorization: Bearer {token}
```

**Resposta:**
```json
[
  {
    "id": "44444444-4444-4444-4444-444444444441",
    "name": "Rosca Direta",
    "description": "Exercício fundamental para bíceps"
  },
  {
    "id": "44444444-4444-4444-4444-444444444442",
    "name": "Rosca Alternada",
    "description": "Trabalho alternado dos bíceps"
  },
  {
    "id": "44444444-4444-4444-4444-444444444443",
    "name": "Rosca Martelo",
    "description": "Trabalha bíceps e antebraço"
  }
]
```

**Nota:** Apenas exercícios que:
1. **NÃO estão** na pasta especificada
2. **Contêm** o termo de busca no nome (case-insensitive)

---

## 💻 Exemplos de Uso

### Front-end: Buscar Exercícios Disponíveis

```typescript
// Exemplo: Buscar exercícios disponíveis para uma pasta
async function searchAvailableExercises(
  workoutFolderId: string,
  searchTerm: string
) {
  const response = await fetch(
    `http://localhost:8080/api/exercises/available/${workoutFolderId}/search?searchTerm=${encodeURIComponent(searchTerm)}`,
    {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    }
  );

  if (!response.ok) {
    throw new Error('Erro ao buscar exercícios disponíveis');
  }

  const exercises = await response.json();
  return exercises;
}

// Uso:
const exercises = await searchAvailableExercises(
  '550e8400-e29b-41d4-a716-446655440000',
  'supino'
);

// Resultado: Lista de exercícios com "supino" no nome que NÃO estão na pasta
// Exemplo: ["Supino Inclinado", "Supino Declinado"] (se "Supino Reto" já estiver na pasta)
```

### Front-end: Listar Todos os Exercícios Disponíveis

```typescript
// Exemplo: Listar todos os exercícios disponíveis para uma pasta
async function getAvailableExercises(workoutFolderId: string) {
  const response = await fetch(
    `http://localhost:8080/api/exercises/available/${workoutFolderId}`,
    {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    }
  );

  if (!response.ok) {
    throw new Error('Erro ao listar exercícios disponíveis');
  }

  const exercises = await response.json();
  return exercises;
}

// Uso:
const allAvailableExercises = await getAvailableExercises(
  '550e8400-e29b-41d4-a716-446655440000'
);

// Resultado: Todos os exercícios que NÃO estão na pasta
```

### Fluxo Completo: Adicionar Exercício a uma Pasta

```typescript
// 1. Obter exercícios disponíveis para a pasta
const availableExercises = await getAvailableExercises(workoutFolderId);

// 2. Filtrar por nome (opcional, ou usar o endpoint de busca)
const filteredExercises = availableExercises.filter(ex =>
  ex.name.toLowerCase().includes('supino')
);

// 3. Usuário seleciona um exercício
const selectedExercise = filteredExercises[0];

// 4. Adicionar exercício à pasta
const response = await fetch(
  `http://localhost:8080/api/workout-folders/${workoutFolderId}/exercises`,
  {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      exerciseId: selectedExercise.id,
      order: 1
    })
  }
);

// 5. Após adicionar, o exercício não aparecerá mais na lista de disponíveis
```

---

## 🔧 Implementação Técnica

### Specifications Criadas

#### 1. `SearchExercisesByNameSpec` (Atualizada)
- **Localização:** `src/GymDogs.Domain/Exercises/Specification/SearchExercisesByNameSpec.cs`
- **Funcionalidade:** Busca exercícios por nome (case-insensitive)
- **Implementação:**
  ```csharp
  var lowerSearchTerm = searchTerm.ToLowerInvariant();
  Query.Where(e => e.Name.ToLower().Contains(lowerSearchTerm))
       .AsNoTracking()
       .OrderBy(e => e.Name);
  ```

#### 2. `GetAvailableExercisesForFolderSpec` (Nova)
- **Localização:** `src/GymDogs.Domain/Exercises/Specification/GetAvailableExercisesForFolderSpec.cs`
- **Funcionalidade:** Lista exercícios que não estão em uma pasta específica
- **Implementação:**
  ```csharp
  Query.Where(e => !e.FolderExercises.Any(fe => fe.WorkoutFolderId == workoutFolderId))
       .AsNoTracking()
       .OrderBy(e => e.Name);
  ```

#### 3. `SearchAvailableExercisesForFolderSpec` (Nova)
- **Localização:** `src/GymDogs.Domain/Exercises/Specification/SearchAvailableExercisesForFolderSpec.cs`
- **Funcionalidade:** Busca exercícios disponíveis por nome (case-insensitive)
- **Implementação:**
  ```csharp
  var lowerSearchTerm = searchTerm.ToLowerInvariant();
  Query.Where(e => !e.FolderExercises.Any(fe => fe.WorkoutFolderId == workoutFolderId) &&
                  e.Name.ToLower().Contains(lowerSearchTerm))
       .AsNoTracking()
       .OrderBy(e => e.Name);
  ```

### Queries Criadas

#### 1. `GetAvailableExercisesForFolderQuery`
- **Localização:** `src/GymDogs.Application/Exercises/Queries/GetAvailableExercisesForFolderQuery.cs`
- **Handler:** `GetAvailableExercisesForFolderQueryHandler`
- **Validação:** Verifica se `WorkoutFolderId` não é `Guid.Empty`

#### 2. `SearchAvailableExercisesForFolderQuery`
- **Localização:** `src/GymDogs.Application/Exercises/Queries/SearchAvailableExercisesForFolderQuery.cs`
- **Handler:** `SearchAvailableExercisesForFolderQueryHandler`
- **Validação:** Verifica se `WorkoutFolderId` não é `Guid.Empty` e se `SearchTerm` não está vazio

### Factory Pattern

As specifications são criadas através do `ISpecificationFactory`:

```csharp
// Interface
GetAvailableExercisesForFolderSpec CreateGetAvailableExercisesForFolderSpec(Guid workoutFolderId);
SearchAvailableExercisesForFolderSpec CreateSearchAvailableExercisesForFolderSpec(Guid workoutFolderId, string searchTerm);

// Implementação
public GetAvailableExercisesForFolderSpec CreateGetAvailableExercisesForFolderSpec(Guid workoutFolderId)
{
    return new GetAvailableExercisesForFolderSpec(workoutFolderId);
}

public SearchAvailableExercisesForFolderSpec CreateSearchAvailableExercisesForFolderSpec(Guid workoutFolderId, string searchTerm)
{
    var normalizedSearchTerm = searchTerm?.Trim() ?? string.Empty;
    return new SearchAvailableExercisesForFolderSpec(workoutFolderId, normalizedSearchTerm);
}
```

### Controller

Endpoints adicionados em `ExercisesController`:

```csharp
[HttpGet("available/{workoutFolderId}")]
public async Task<ActionResult<IEnumerable<GetExerciseDto>>> GetAvailableExercisesForFolder(
    [FromRoute] Guid workoutFolderId,
    CancellationToken cancellationToken)

[HttpGet("available/{workoutFolderId}/search")]
public async Task<ActionResult<IEnumerable<GetExerciseDto>>> SearchAvailableExercisesForFolder(
    [FromRoute] Guid workoutFolderId,
    [FromQuery] string searchTerm,
    CancellationToken cancellationToken)
```

---

## 🐛 Troubleshooting

### Problema: Busca não encontra exercícios (case-sensitive)

**Sintoma:** Buscar "supino" não encontra "Supino Reto"

**Causa:** A busca ainda está case-sensitive

**Solução:**
1. Verifique se a migration foi aplicada
2. Verifique se o código está usando `ToLower()` na specification
3. Rebuild o projeto: `dotnet build`

### Problema: Exercícios já adicionados aparecem na lista de disponíveis

**Sintoma:** Exercício que já está na pasta aparece na lista de disponíveis

**Causa:** Problema na query ou relacionamento não carregado

**Solução:**
1. Verifique se o `WorkoutFolderId` está correto
2. Verifique se o relacionamento `FolderExercises` está sendo carregado
3. Verifique os logs do banco de dados para ver a query SQL gerada

### Problema: Endpoint retorna 400 Bad Request

**Sintoma:** `GET /api/exercises/available/{id}` retorna 400

**Causa:** ID inválido ou termo de busca vazio

**Solução:**
1. Verifique se o `workoutFolderId` é um GUID válido
2. Verifique se o `searchTerm` não está vazio (para endpoint de busca)
3. Verifique os logs para ver a mensagem de erro específica

### Problema: Performance lenta na busca

**Sintoma:** Busca demora muito para retornar resultados

**Causa:** Falta de índices ou query ineficiente

**Solução:**
1. Verifique se há índice na coluna `Name` da tabela `Exercises`
2. Verifique se há índice na coluna `WorkoutFolderId` da tabela `FolderExercises`
3. Considere adicionar índices compostos se necessário

---

## 📊 Performance

### Otimizações Implementadas

1. **`AsNoTracking()`**: Todas as queries de leitura usam `AsNoTracking()` para melhor performance
2. **Índices**: A tabela `Exercises` tem índice na coluna `Name`
3. **Subquery Eficiente**: A verificação de exercícios na pasta usa `Any()`, que é traduzido para uma subquery eficiente no PostgreSQL

### Query SQL Gerada (Exemplo)

```sql
-- Busca exercícios disponíveis com filtro de nome
SELECT e."Id", e."Name", e."Description", e."CreatedAt", e."LastUpdatedAt"
FROM "Exercises" AS e
WHERE NOT EXISTS (
    SELECT 1
    FROM "FolderExercises" AS fe
    WHERE fe."WorkoutFolderId" = @workoutFolderId
      AND fe."ExerciseId" = e."Id"
)
AND LOWER(e."Name") LIKE LOWER(@searchTerm)
ORDER BY e."Name"
```

---

## ✅ Checklist de Testes

### Busca Case-Insensitive

- [ ] Buscar "supino" encontra "Supino Reto"
- [ ] Buscar "SUPINO" encontra "Supino Reto"
- [ ] Buscar "SuPiNo" encontra "Supino Reto"
- [ ] Buscar "rosca" encontra "Rosca Direta"
- [ ] Buscar "ROSCA" encontra "Rosca Direta"

### Exercícios Disponíveis

- [ ] Listar exercícios disponíveis retorna apenas exercícios não adicionados
- [ ] Após adicionar exercício, ele não aparece mais na lista
- [ ] Buscar exercícios disponíveis funciona corretamente
- [ ] Busca case-insensitive funciona nos exercícios disponíveis
- [ ] Pasta vazia retorna todos os exercícios

### Validações

- [ ] `WorkoutFolderId` vazio retorna 400
- [ ] `SearchTerm` vazio retorna 400 (no endpoint de busca)
- [ ] `WorkoutFolderId` inválido retorna 400
- [ ] Autenticação requerida (401 se não autenticado)

---

## 📚 Referências

- [Entity Framework Core - Case-Insensitive Search](https://learn.microsoft.com/en-us/ef/core/querying/filters)
- [PostgreSQL - ILIKE Operator](https://www.postgresql.org/docs/current/functions-matching.html)
- [Ardalis Specification Pattern](https://github.com/ardalis/Specification)

---

**Última atualização:** Janeiro 2024  
**Versão:** 1.0.0
