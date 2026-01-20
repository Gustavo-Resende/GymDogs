# GymDogs

## 🎯 Objetivo do Sistema

O **GymDogs** é uma aplicação para gerenciamento e acompanhamento de treinos de academia. O sistema permite que usuários organizem seus exercícios em pastas personalizadas, registrem séries com pesos e repetições, e compartilhem seus treinos com outros usuários através de perfis públicos ou privados.

### Principais Funcionalidades

- **Gestão de Perfis**: Cada usuário possui um perfil com controle de visibilidade (público/privado)
- **Organização por Pastas**: Crie pastas de treino personalizadas (ex: "Costas", "Peito", "Pernas")
- **Catálogo de Exercícios**: Sistema centralizado de exercícios que podem ser reutilizados
- **Registro de Séries**: Controle detalhado de séries, repetições e cargas levantadas
- **Acompanhamento de Progresso**: Histórico completo de treinos para análise de evolução

---

## 📋 Regras de Negócio

### Usuário (User)

- Cada usuário deve ter um **username** único e obrigatório (máximo 100 caracteres)
- Cada usuário deve ter um **email** único e obrigatório (máximo 255 caracteres)
- A senha deve ser armazenada como **hash** usando BCrypt
- Ao criar um usuário, um **perfil é automaticamente criado** (relacionamento 1:1)
- Username e email são normalizados (trim e lower case para email) antes de serem salvos

### Perfil (Profile)

- Um perfil é **automaticamente criado** quando um usuário é cadastrado
- Cada perfil está vinculado a **exatamente um usuário** (relacionamento 1:1)
- O perfil possui **visibilidade** que pode ser:
  - **Público**: Visível para todos os usuários
  - **Privado**: Visível apenas para o próprio usuário
- O **displayName** é obrigatório (máximo 200 caracteres), padrão é string vazia
- A **bio** é opcional (máximo 1000 caracteres)
- Um perfil pode ter **múltiplas pastas de treino** (relacionamento 1:N)

### Pasta de Treino (WorkoutFolder)

- Pertence a **um perfil específico** (relacionamento N:1)
- Possui um **nome** obrigatório (máximo 200 caracteres)
- Possui uma **descrição** opcional
- Possui um campo **Order** para controlar a ordem de exibição (deve ser ≥ 0)
- Uma pasta pode conter **múltiplos exercícios** através do relacionamento com FolderExercise

### Exercício (Exercise)

- Exercícios são **criados no catálogo global** e podem ser reutilizados
- Possui um **nome** obrigatório (máximo 200 caracteres)
- Possui uma **descrição** opcional (máximo 1000 caracteres)
- Um exercício pode estar presente em **múltiplas pastas de treino** através de FolderExercise

### Exercício na Pasta (FolderExercise)

- Representa a **associação de um exercício a uma pasta de treino**
- Previne duplicação: o **mesmo exercício não pode ser adicionado duas vezes na mesma pasta**
- Possui um campo **Order** para controlar a ordem dos exercícios dentro da pasta (deve ser ≥ 0)
- Um FolderExercise pode ter **múltiplas séries** (relacionamento 1:N com ExerciseSet)

### Série (ExerciseSet)

- Representa **uma série executada** de um exercício em uma pasta
- Pertence a **um FolderExercise específico** (relacionamento N:1)
- Possui um **SetNumber** que identifica o número da série (deve ser > 0)
  - Se não fornecido, o sistema **calcula automaticamente** o próximo número disponível
- **Reps** (repetições) deve ser entre 1 e 1000 (obrigatório, deve ser > 0)
- **Weight** (peso) deve ser entre 0 e 10.000 kg (deve ser ≥ 0)
- Mantém **histórico completo** de todas as séries executadas para acompanhamento de progresso

---

## 🏗️ Estrutura de Entidades

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

### Relacionamentos

1. **User ↔ Profile**: Relacionamento 1:1
   - Ao deletar um User, o Profile é deletado em cascata
   - Um User sempre possui um Profile

2. **Profile ↔ WorkoutFolder**: Relacionamento 1:N
   - Um Profile pode ter múltiplas WorkoutFolders
   - Ao deletar um Profile, todas as WorkoutFolders são deletadas em cascata

3. **WorkoutFolder ↔ FolderExercise**: Relacionamento 1:N
   - Uma WorkoutFolder pode ter múltiplos FolderExercises
   - Ao deletar uma WorkoutFolder, todos os FolderExercises são deletados em cascata

4. **Exercise ↔ FolderExercise**: Relacionamento 1:N
   - Um Exercise pode estar em múltiplos FolderExercises (reutilização)
   - Ao deletar um Exercise, todos os FolderExercises relacionados são deletados em cascata

5. **FolderExercise ↔ ExerciseSet**: Relacionamento 1:N
   - Um FolderExercise pode ter múltiplas ExerciseSets (histórico de séries)
   - Ao deletar um FolderExercise, todas as ExerciseSets são deletadas em cascata

---

## 🔄 Fluxo de Uso

### Cenário 1: Criação de Usuário e Primeiro Treino

1. **Criar Usuário**
   - Sistema cria automaticamente um Profile vinculado
   - Profile inicialmente é **público** por padrão

2. **Criar Pasta de Treino**
   - Usuário cria uma WorkoutFolder (ex: "Treino A - Costas")
   - Define nome, descrição opcional e ordem

3. **Adicionar Exercícios ao Catálogo** (se ainda não existirem)
   - Criar Exercise no catálogo global (ex: "Puxada Frontal")
   - Pode ser reutilizado em outras pastas

4. **Adicionar Exercício à Pasta**
   - Criar FolderExercise associando Exercise à WorkoutFolder
   - Define a ordem do exercício na pasta

5. **Registrar Séries**
   - Criar ExerciseSet para cada série executada
   - Sistema calcula automaticamente o SetNumber se não fornecido
   - Registra reps e weight de cada série

### Cenário 2: Visualização de Perfil Público

- Usuários podem **visualizar perfis públicos** de outros usuários
- Perfis privados são visíveis apenas para o próprio usuário
- Ao visualizar um perfil público, é possível ver:
  - Pastas de treino do usuário
  - Exercícios em cada pasta
  - Histórico de séries executadas

### Cenário 3: Acompanhamento de Progresso

- Todas as **ExerciseSets** são mantidas como histórico
- Permite comparar séries ao longo do tempo
- Facilita análise de evolução de cargas e repetições

---

## 🛡️ Validações e Proteções

### Domain Layer (GuardClauses)

Todas as entidades utilizam **Ardalis.GuardClauses** para garantir invariantes:

- **Campos obrigatórios**: Validados contra null/whitespace
- **Limites de tamanho**: Validados contra tamanho máximo permitido
- **Valores numéricos**: Validados contra negativos ou fora de range
- **Enums**: Validados contra valores inválidos
- **Guids**: Validados contra valores vazios (Guid.Empty)

### Application Layer

- **Validação de duplicação**: Verifica se email/username já existem antes de criar usuário
- **Validação de existência**: Verifica se entidades relacionadas existem antes de criar associações
- **Prevenção de duplicação**: Impede adicionar o mesmo exercício duas vezes na mesma pasta
- **Normalização de dados**: Aplica trim e lower case quando necessário

### Error Handling

- **Middleware global** captura exceções e retorna respostas estruturadas
- **Domain exceptions** são convertidas para `Ardalis.Result` formatado
- Respostas de erro seguem padrão consistente com status HTTP apropriado

---

## 📊 Conceitos Arquiteturais

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

---

## 🎨 Funcionalidades Futuras (Roadmap)

- Sistema de grupos para compartilhamento de treinos
- Feed de atividades (exercícios recentes de perfis que você segue)
- Gráficos e estatísticas de progresso
- Fotos e vídeos de exercícios
- Sistema de notificações
- Planejamento de treinos semanais/mensais