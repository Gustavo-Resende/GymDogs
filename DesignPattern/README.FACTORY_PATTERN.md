# 🏭 Factory Pattern - Explicação Didática

## 📚 O que é o Factory Pattern?

O **Factory Pattern** (Padrão de Fábrica) é um padrão de design criacional que fornece uma interface para criar objetos sem especificar exatamente qual classe será instanciada. Ele encapsula a lógica de criação de objetos, centralizando e padronizando o processo.

---

## 🌍 Analogia do Mundo Real: Fábrica de Carros

Imagine uma **fábrica de carros**. Quando você quer comprar um carro, você não vai até a linha de produção e monta o carro você mesmo. Você vai até a **concessionária** (a fábrica) e pede:

- "Quero um carro esportivo"
- "Quero um carro familiar"
- "Quero um carro elétrico"

A **concessionária** (Factory) sabe:
- Como montar cada tipo de carro
- Quais peças usar
- Como configurar tudo corretamente
- Como garantir qualidade

Você não precisa saber:
- Como soldar as peças
- Como instalar o motor
- Como configurar o sistema elétrico

Você só pede e recebe o carro pronto!

No Factory Pattern:
- **A Factory** = A concessionária (sabe como criar objetos)
- **Os Produtos** = Os carros (as Specifications)
- **O Cliente** = Você (os Handlers que pedem as Specifications)

---

## 🏗️ Como Funciona no Código?

### Estrutura do Pattern

```
┌─────────────────────────────────────┐
│   Client (Command/Query Handler)     │
│   - Precisa de uma Specification     │
│   - Não sabe como criar              │
└──────────────┬──────────────────────┘
               │
               │ pede
               ▼
┌─────────────────────────────────────┐
│   Factory (ISpecificationFactory)   │
│   - Sabe como criar Specifications  │
│   - Centraliza lógica de criação    │
└──────────────┬──────────────────────┘
               │
               │ cria
               ▼
┌─────────────────────────────────────┐
│   Product (Specification)            │
│   - GetUserByEmailSpec              │
│   - GetProfileByIdSpec               │
│   - etc.                             │
└─────────────────────────────────────┘
```

---

## 📍 Onde Aplicamos no Projeto GymDogs?

### Problema Anterior

Antes, cada Handler criava Specifications diretamente, com normalização repetida:

```csharp
// ❌ ANTES: Criação direta com normalização repetida
public async Task<Result<CreateUserDto>> Handle(...)
{
    // Normalização repetida em TODOS os handlers
    var emailNormalized = request.Email?.Trim().ToLowerInvariant() ?? string.Empty;
    var usernameNormalized = request.Username?.Trim() ?? string.Empty;
    
    // Criação direta da Specification
    var existingUser = await _userRepository.FirstOrDefaultAsync(
        new GetUserByEmailSpec(emailNormalized), // Normalização manual
        cancellationToken);
}
```

**Problemas:**
- ❌ Normalização duplicada em vários lugares (trim, toLowerInvariant)
- ❌ Difícil de testar (precisa mockar a Specification)
- ❌ Difícil de manter (se mudar a normalização, precisa mudar em vários lugares)
- ❌ Viola DRY (Don't Repeat Yourself)

### Solução com Factory Pattern

Agora, a Factory centraliza a criação e normalização:

```csharp
// ✅ DEPOIS: Factory centraliza tudo
public async Task<Result<CreateUserDto>> Handle(...)
{
    // Factory faz a normalização internamente
    var existingUser = await _userRepository.FirstOrDefaultAsync(
        _specificationFactory.CreateGetUserByEmailSpec(request.Email), // Normalização automática
        cancellationToken);
}
```

**Benefícios:**
- ✅ Normalização centralizada (um lugar só)
- ✅ Fácil de testar (mocka o Factory)
- ✅ Fácil de manter (muda em um lugar só)
- ✅ Respeita DRY

---

## 🔍 Parte por Parte: Como Implementamos

### 1️⃣ A Interface (Contrato da Fábrica)

**Arquivo:** `src/GymDogs.Application/Common/Specification/ISpecificationFactory.cs`

```csharp
public interface ISpecificationFactory
{
    GetUserByEmailSpec CreateGetUserByEmailSpec(string email);
    GetUserByUsernameSpec CreateGetUserByUsernameSpec(string username);
    // ... outras Specifications
}
```

**Analogia:** É como o "catálogo" da concessionária. Lista todos os tipos de carros que a fábrica pode produzir.

**Por quê?**
- Define quais Specifications podem ser criadas
- Garante que todos os Handlers usam o mesmo método
- Facilita mock em testes

---

### 2️⃣ A Implementação (A Fábrica Real)

**Arquivo:** `src/GymDogs.Infrastructure/Persistence/Specification/SpecificationFactory.cs`

```csharp
public class SpecificationFactory : ISpecificationFactory
{
    public GetUserByEmailSpec CreateGetUserByEmailSpec(string email)
    {
        // Normalização centralizada
        var normalizedEmail = email?.Trim().ToLowerInvariant() ?? string.Empty;
        return new GetUserByEmailSpec(normalizedEmail);
    }
    
    public GetUserByUsernameSpec CreateGetUserByUsernameSpec(string username)
    {
        // Normalização centralizada
        var normalizedUsername = username?.Trim() ?? string.Empty;
        return new GetUserByUsernameSpec(normalizedUsername);
    }
}
```

**Analogia:** É a "linha de produção" da fábrica. Ela sabe:
- Como montar cada tipo de carro (criar cada Specification)
- Quais ajustes fazer (normalização: trim, lowercase)
- Como garantir qualidade (validações, se necessário)

**O que faz:**
- Centraliza a lógica de normalização (trim, toLowerInvariant)
- Cria as Specifications com os dados já normalizados
- Garante consistência em todo o código

---

### 3️⃣ Uso no Handler (O Cliente)

**Arquivo:** `src/GymDogs.Application/Users/Commands/CreateUserCommand.cs`

```csharp
internal class CreateUserCommandHandler
{
    private readonly ISpecificationFactory _specificationFactory; // Factory injetada

    public async Task<Result<CreateUserDto>> Handle(...)
    {
        // ANTES: Normalização manual + criação direta
        // var emailNormalized = request.Email?.Trim().ToLowerInvariant() ?? string.Empty;
        // var existingUser = await _userRepository.FirstOrDefaultAsync(
        //     new GetUserByEmailSpec(emailNormalized), cancellationToken);

        // DEPOIS: Factory faz tudo
        var existingUser = await _userRepository.FirstOrDefaultAsync(
            _specificationFactory.CreateGetUserByEmailSpec(request.Email), // Normalização automática!
            cancellationToken);
    }
}
```

**Analogia:** É como você indo à concessionária e pedindo: "Quero um carro esportivo". Você não precisa saber como montar, só pede e recebe.

**O que faz:**
- Pede a Specification para o Factory
- Não precisa fazer normalização manual
- Código mais limpo e legível

---

### 4️⃣ Registro no Dependency Injection

**Arquivo:** `src/GymDogs.Presentation/Program.cs`

```csharp
// Registra a Factory no DI Container
builder.Services.AddScoped<ISpecificationFactory, SpecificationFactory>();
```

**Analogia:** É como "contratar" a fábrica. Você registra ela no sistema, e quando alguém precisa de um carro, o sistema entrega a fábrica para ela usar.

---

## 🎯 Fluxo Completo: Como Funciona na Prática

### Cenário: Handler precisa buscar usuário por email

```
1. Handler recebe request
   └─> CreateUserCommand { Email = "  JOHN@EXAMPLE.COM  " }

2. Handler pede Specification para o Factory
   └─> _specificationFactory.CreateGetUserByEmailSpec(request.Email)

3. Factory recebe o email "cru"
   └─> "  JOHN@EXAMPLE.COM  "

4. Factory normaliza o email
   └─> Trim() → "JOHN@EXAMPLE.COM"
   └─> ToLowerInvariant() → "john@example.com"

5. Factory cria a Specification com email normalizado
   └─> new GetUserByEmailSpec("john@example.com")

6. Factory retorna a Specification pronta
   └─> GetUserByEmailSpec(email: "john@example.com")

7. Handler usa a Specification no Repository
   └─> _userRepository.FirstOrDefaultAsync(specification, ...)

8. Repository executa a query com email normalizado
   └─> SELECT * FROM Users WHERE Email = 'john@example.com'
```

---

## ✅ Benefícios do Factory Pattern

### 1. **DRY (Don't Repeat Yourself)**
- ✅ Normalização em um lugar só
- ✅ Não repete código de criação

**Exemplo:** Se precisar mudar a normalização de email (ex: remover espaços no meio), muda só no Factory.

### 2. **Single Responsibility Principle**
- Factory: responsável por criar Specifications
- Handler: responsável por orquestrar a lógica de negócio

### 3. **Testabilidade**
- Fácil mockar o Factory em testes
- Testa a normalização isoladamente

```csharp
// Teste do Factory
[Fact]
public void CreateGetUserByEmailSpec_ShouldNormalizeEmail()
{
    var factory = new SpecificationFactory();
    var spec = factory.CreateGetUserByEmailSpec("  JOHN@EXAMPLE.COM  ");
    
    // Verifica que a normalização foi feita
    // (precisa acessar a propriedade interna ou testar o comportamento)
}
```

### 4. **Manutenibilidade**
- Mudanças centralizadas
- Fácil encontrar onde Specifications são criadas

### 5. **Consistência**
- Todas as Specifications são criadas da mesma forma
- Normalização sempre aplicada

---

## 📊 Comparação: Antes vs. Depois

| Aspecto | ❌ Antes (Criação Direta) | ✅ Depois (Factory) |
|---------|---------------------------|---------------------|
| **Normalização** | Repetida em cada Handler | Centralizada no Factory |
| **Testabilidade** | Difícil (cria objetos reais) | Fácil (mocka o Factory) |
| **Manutenibilidade** | Muda em vários lugares | Muda em um lugar só |
| **DRY** | ❌ Viola | ✅ Respeita |
| **Consistência** | Pode variar | Sempre igual |

---

## 🧪 Como Testar

### Teste do Factory

```csharp
[Fact]
public void CreateGetUserByEmailSpec_ShouldNormalizeEmail()
{
    // Arrange
    var factory = new SpecificationFactory();
    var email = "  JOHN@EXAMPLE.COM  ";

    // Act
    var spec = factory.CreateGetUserByEmailSpec(email);

    // Assert
    // Verifica que a Specification foi criada corretamente
    // (testa o comportamento através do Repository ou acessa propriedades internas)
}
```

### Teste do Handler com Factory Mockado

```csharp
[Fact]
public void Handle_ShouldUseFactoryToCreateSpecification()
{
    // Arrange
    var factoryMock = new Mock<ISpecificationFactory>();
    var spec = new GetUserByEmailSpec("john@example.com");
    factoryMock.Setup(f => f.CreateGetUserByEmailSpec(It.IsAny<string>()))
               .Returns(spec);
    
    var handler = new CreateUserCommandHandler(..., factoryMock.Object);

    // Act
    var result = await handler.Handle(new CreateUserCommand(...), CancellationToken.None);

    // Assert
    factoryMock.Verify(f => f.CreateGetUserByEmailSpec(It.IsAny<string>()), Times.Once);
}
```

---

## 🎓 Resumo: O que Aprendemos?

1. **Factory Pattern** = Centraliza criação de objetos
2. **Interface** = Define quais objetos podem ser criados
3. **Implementação** = Faz a criação real com lógica centralizada
4. **Cliente** = Usa o Factory sem saber como criar
5. **Benefícios** = DRY, testabilidade, manutenibilidade, consistência

---

## 📁 Estrutura de Arquivos Criada

```
src/GymDogs.Application/
└── Common/
    └── Specification/
        └── ISpecificationFactory.cs          (Interface)

src/GymDogs.Infrastructure/
└── Persistence/
    └── Specification/
        └── SpecificationFactory.cs          (Implementação)

src/GymDogs.Application/
└── Users/
    └── Commands/
        └── CreateUserCommand.cs             (Refatorado para usar Factory)

README.FACTORY_PATTERN.md                    (Documentação)
```

---

## 🔄 Onde Mais Podemos Aplicar?

O Factory Pattern pode ser aplicado em outros handlers também:

- ✅ `LoginCommand` - usa `GetUserByEmailSpec`
- ✅ `UpdateUserCommand` - usa `GetUserByUsernameSpec` e `GetUserByEmailSpec`
- ✅ `SearchPublicProfilesQuery` - usa `SearchPublicProfilesSpec`
- ✅ `SearchExercisesByNameQuery` - usa `SearchExercisesByNameSpec`
- ✅ E muitos outros...

**Próximo passo:** Refatorar gradualmente os outros handlers para usar o Factory também!

---

## 🚀 Próximos Passos

Agora que você entendeu o Factory Pattern, podemos:

1. **Refatorar outros Handlers** para usar o Factory
2. **Implementar Builder Pattern** para JWT Tokens
3. **Adicionar mais normalizações** no Factory (validações, transformações)

Quer que eu refatore mais Handlers ou prefere implementar o Builder Pattern agora? 🎯
