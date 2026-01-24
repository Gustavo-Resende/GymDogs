# 🏗️ Builder Pattern - Documentação Completa

## 📚 Índice

1. [O que é o Builder Pattern?](#-o-que-é-o-builder-pattern)
2. [Analogia do Mundo Real](#-analogia-do-mundo-real)
3. [Problema que Resolve](#-problema-que-resolve)
4. [Estrutura do Pattern](#-estrutura-do-pattern)
5. [Onde Aplicamos no Projeto?](#-onde-aplicamos-no-projeto-gymdogs)
6. [Implementação Detalhada](#-implementação-detalhada)
7. [Fluxo Completo](#-fluxo-completo-como-funciona-na-prática)
8. [Benefícios](#-benefícios)
9. [Comparação: Antes vs Depois](#-comparação-antes-vs-depois)
10. [Referências](#-referências)

---

## 🎯 O que é o Builder Pattern?

O **Builder Pattern** é um padrão de design criacional que permite construir objetos complexos passo a passo. Ao invés de passar muitos parâmetros ou usar construtores longos, você monta o objeto de forma fluente e legível.

**Definição técnica:** O Builder Pattern separa a construção de um objeto complexo da sua representação, permitindo que o mesmo processo de construção crie diferentes representações.

**Referência:** [Refactoring.Guru - Builder Pattern](https://refactoring.guru/design-patterns/builder)

---

## 🏠 Analogia do Mundo Real

Imagine que você está **construindo uma casa**:

### ❌ Sem Builder (Método Tradicional)

```csharp
// Você precisa passar TODOS os parâmetros de uma vez
var casa = new Casa(
    quartos: 3,
    banheiros: 2,
    garagem: true,
    piscina: false,
    jardim: true,
    andares: 2,
    cor: "Branco",
    material: "Concreto",
    area: 200,
    // ... mais 10 parâmetros
);

// Problemas:
// - Difícil lembrar a ordem dos parâmetros
// - E se você não quiser piscina? Precisa passar false mesmo assim
// - E se quiser só alguns parâmetros? Precisa passar null ou valores padrão
// - Código difícil de ler e manter
```

### ✅ Com Builder (Método Fluente)

```csharp
// Você constrói passo a passo, só o que precisa
var casa = new CasaBuilder()
    .ComQuartos(3)
    .ComBanheiros(2)
    .ComGaragem()        // Método sem parâmetro = true
    .ComJardim()         // Método sem parâmetro = true
    .ComAndares(2)
    .ComCor("Branco")
    .Build();            // Só no final você "constrói" a casa

// Vantagens:
// - Legível: cada linha explica o que está sendo adicionado
// - Flexível: adiciona só o que precisa
// - Ordem não importa (na maioria dos casos)
// - Fácil de estender: adicionar novos métodos não quebra código existente
```

**Analogia:** É como pedir um hambúrguer no McDonald's. Você não diz "Quero um hambúrguer com pão, carne, alface, tomate, queijo, molho, sem cebola, sem picles". Você diz: "Quero um hambúrguer, com alface, tomate, queijo, sem cebola". O Builder funciona assim: você vai adicionando os "ingredientes" que quer, na ordem que quiser.

---

## 🔍 Problema que Resolve

### Problema 1: **Muitos Parâmetros (Long Parameter List)**

Quando um método precisa de muitos parâmetros, fica difícil:
- Lembrar a ordem correta
- Entender o que cada parâmetro faz
- Manter o código legível

```csharp
// ❌ PROBLEMA: 5 parâmetros, difícil de ler
var token = GenerateToken(userId, username, email, role, expirationMinutes);
// Qual é a ordem? O que cada um faz?
```

### Problema 2: **Dificuldade de Extensão**

Adicionar novos parâmetros quebra código existente:

```csharp
// ❌ Se eu quiser adicionar um claim customizado, preciso mudar a assinatura
var token = GenerateToken(userId, username, email, role, expirationMinutes, customClaim);
// Isso quebra TODOS os lugares que chamam esse método!
```

### Problema 3: **Validação Espalhada**

Validações ficam espalhadas pelo código:

```csharp
// ❌ Validações em vários lugares
if (userId == Guid.Empty) throw new Exception("...");
if (string.IsNullOrEmpty(username)) throw new Exception("...");
// ...
```

### Problema 4: **Código Repetitivo**

Mesma lógica repetida em vários lugares:

```csharp
// ❌ Mesma chamada repetida em LoginCommand e RefreshTokenCommand
var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Username, user.Email, ...);
```

---

## 🏗️ Estrutura do Pattern

```
┌─────────────────────────────────────────────────────────┐
│                    Client (Handler)                      │
│  - Usa o Builder para construir objetos complexos      │
└──────────────────────┬──────────────────────────────────┘
                       │
                       │ usa
                       ▼
┌─────────────────────────────────────────────────────────┐
│              Builder Interface (IJwtTokenBuilder)        │
│  - Define métodos fluentes (WithXxx)                    │
│  - Define método Build()                                 │
└──────────────────────┬──────────────────────────────────┘
                       │
                       │ implementa
                       ▼
┌─────────────────────────────────────────────────────────┐
│           Concrete Builder (JwtTokenBuilder)              │
│  - Armazena estado (campos privados)                     │
│  - Implementa métodos WithXxx()                         │
│  - Implementa Build() com validação e construção        │
└─────────────────────────────────────────────────────────┘
                       │
                       │ constrói
                       ▼
┌─────────────────────────────────────────────────────────┐
│              Product (JWT Token String)                 │
│  - Objeto complexo final                                │
│  - Criado apenas quando Build() é chamado                │
└─────────────────────────────────────────────────────────┘
```

**Componentes:**

1. **Builder Interface (`IJwtTokenBuilder`)**
   - Define a interface com métodos fluentes
   - Cada método retorna `this` para permitir encadeamento
   - Método `Build()` finaliza a construção

2. **Concrete Builder (`JwtTokenBuilder`)**
   - Armazena o estado em campos privados
   - Implementa os métodos `WithXxx()`
   - Valida e constrói o objeto no `Build()`

3. **Product (JWT Token String)**
   - O objeto complexo final
   - Só é criado quando `Build()` é chamado

4. **Client (Handlers)**
   - Usa o Builder para construir objetos
   - Não precisa conhecer os detalhes de construção

---

## 📍 Onde Aplicamos no Projeto GymDogs?

### Problema Anterior

Antes, a geração de JWT Tokens estava assim:

```csharp
// ❌ ANTES: Muitos parâmetros, difícil de ler e manter
public string GenerateToken(
    Guid userId,           // Parâmetro 1
    string username,       // Parâmetro 2
    string email,          // Parâmetro 3
    string role,           // Parâmetro 4
    int? expirationMinutes // Parâmetro 5
)
```

**Uso no código:**

```csharp
// LoginCommand.cs
var token = _jwtTokenGenerator.GenerateToken(
    user.Id, 
    user.Username, 
    user.Email, 
    user.Role.ToString(),
    accessTokenExpirationMinutes);
```

**Problemas:**
- ❌ 5 parâmetros - difícil lembrar a ordem
- ❌ Difícil adicionar claims customizados
- ❌ Validações espalhadas
- ❌ Código repetitivo em vários lugares
- ❌ Não é extensível

### Solução com Builder Pattern

Agora, a construção é fluente e legível:

```csharp
// ✅ DEPOIS: Construção passo a passo, legível e extensível
var token = _jwtTokenBuilder
    .WithUserId(user.Id)
    .WithUsername(user.Username)
    .WithEmail(user.Email)
    .WithRole(user.Role.ToString())
    .WithExpirationMinutes(accessTokenExpirationMinutes)
    .WithCustomClaim("premium", "true")  // Fácil adicionar novos claims!
    .Build();
```

**Vantagens:**
- ✅ Legível: cada linha explica o que está sendo adicionado
- ✅ Extensível: fácil adicionar novos claims sem quebrar código
- ✅ Validação centralizada no `Build()`
- ✅ Reutilizável: mesmo builder pode ser usado em diferentes contextos
- ✅ Testável: fácil mockar o builder

---

## 🔧 Implementação Detalhada

### 1️⃣ Interface do Builder

**Arquivo:** `src/GymDogs.Application/Interfaces/IJwtTokenBuilder.cs`

```csharp
public interface IJwtTokenBuilder
{
    IJwtTokenBuilder WithUserId(Guid userId);
    IJwtTokenBuilder WithUsername(string username);
    IJwtTokenBuilder WithEmail(string email);
    IJwtTokenBuilder WithRole(string role);
    IJwtTokenBuilder WithExpirationMinutes(int minutes);
    IJwtTokenBuilder WithCustomClaim(string type, string value);
    string Build();
}
```

**Analogia:** É como o "cardápio" do Builder. Define o que você pode pedir, mas não como é feito.

**Características:**
- Cada método retorna `IJwtTokenBuilder` para permitir encadeamento (fluent interface)
- Método `Build()` finaliza a construção e retorna o produto final
- Interface permite diferentes implementações (útil para testes)

### 2️⃣ Implementação do Builder

**Arquivo:** `src/GymDogs.Infrastructure/Services/JwtTokenBuilder.cs`

```csharp
public class JwtTokenBuilder : IJwtTokenBuilder
{
    // Estado interno (campos privados)
    private Guid? _userId;
    private string? _username;
    private string? _email;
    private string? _role;
    private int? _expirationMinutes;
    private readonly List<Claim> _customClaims = new();
    private readonly IConfiguration _configuration;

    public JwtTokenBuilder(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // Métodos fluentes - cada um retorna 'this' para encadeamento
    public IJwtTokenBuilder WithUserId(Guid userId)
    {
        _userId = userId;
        return this; // ← Permite encadeamento: .WithX().WithY().WithZ()
    }

    public IJwtTokenBuilder WithUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be null or empty");
        
        _username = username;
        return this;
    }

    // ... outros métodos With...

    // Método Build() - valida e constrói o token
    public string Build()
    {
        // Validações centralizadas
        if (!_userId.HasValue)
            throw new InvalidOperationException("UserId is required");

        // ... outras validações ...

        // Construção do token
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, _userId.Value.ToString()),
            new Claim(ClaimTypes.Name, _username!),
            // ...
        };

        // Adiciona claims customizados
        claims.AddRange(_customClaims);

        // Cria e retorna o token
        var token = new JwtSecurityToken(/* ... */);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

**Analogia:** É a "cozinha" do restaurante. Recebe os pedidos (métodos `WithXxx`), armazena em uma "receita" (campos privados), e quando você pede para "servir" (`Build()`), ela valida, prepara e entrega o prato.

**O que faz:**
- Armazena o estado em campos privados
- Valida cada entrada nos métodos `WithXxx()`
- Valida tudo no `Build()` antes de construir
- Constrói o token apenas quando `Build()` é chamado

### 3️⃣ Uso no Handler (O Cliente)

**Arquivo:** `src/GymDogs.Application/Users/Commands/LoginCommand.cs`

```csharp
internal class LoginCommandHandler
{
    private readonly IJwtTokenBuilder _jwtTokenBuilder; // Builder injetado

    public async Task<Result<LoginDto>> Handle(...)
    {
        // ANTES: Muitos parâmetros, difícil de ler
        // var token = _jwtTokenGenerator.GenerateToken(
        //     user.Id, user.Username, user.Email, user.Role.ToString(), minutes);

        // DEPOIS: Construção fluente e legível
        var token = _jwtTokenBuilder
            .WithUserId(user.Id)
            .WithUsername(user.Username)
            .WithEmail(user.Email)
            .WithRole(user.Role.ToString())
            .WithExpirationMinutes(accessTokenExpirationMinutes)
            .Build();
    }
}
```

**Analogia:** É como você pedindo o hambúrguer. Você vai dizendo o que quer, e no final pede para "servir" (`Build()`).

**O que faz:**
- Usa o Builder para construir o token passo a passo
- Não precisa conhecer os detalhes de como o token é criado
- Código mais legível e fácil de manter

### 4️⃣ Registro no Dependency Injection

**Arquivo:** `src/GymDogs.Presentation/Program.cs`

```csharp
// Builder Pattern: Registro do Builder de JWT Tokens
builder.Services.AddScoped<IJwtTokenBuilder, JwtTokenBuilder>();
```

**Analogia:** É como "contratar" o Builder. Você registra ele no sistema, e quando alguém precisa de um token, o sistema entrega o Builder para ela usar.

---

## 🎯 Fluxo Completo: Como Funciona na Prática

### Passo a Passo

```
1. Handler precisa de um token
   ↓
2. Injeta IJwtTokenBuilder (via DI)
   ↓
3. Chama métodos fluentes para configurar o token:
   .WithUserId(...)
   .WithUsername(...)
   .WithEmail(...)
   .WithRole(...)
   .WithExpirationMinutes(...)
   ↓
4. Chama Build() para finalizar
   ↓
5. Builder valida todos os campos
   ↓
6. Builder constrói o token JWT
   ↓
7. Retorna o token string
   ↓
8. Handler usa o token
```

### Exemplo Completo

```csharp
// 1. Handler recebe o Builder via DI
public LoginCommandHandler(IJwtTokenBuilder jwtTokenBuilder)
{
    _jwtTokenBuilder = jwtTokenBuilder;
}

// 2. Handler usa o Builder para construir o token
public async Task<Result<LoginDto>> Handle(...)
{
    // 3. Configura o token passo a passo
    var token = _jwtTokenBuilder
        .WithUserId(user.Id)                    // ← Adiciona userId
        .WithUsername(user.Username)             // ← Adiciona username
        .WithEmail(user.Email)                   // ← Adiciona email
        .WithRole(user.Role.ToString())          // ← Adiciona role
        .WithExpirationMinutes(15)              // ← Define expiração
        .WithCustomClaim("premium", "true")     // ← Adiciona claim customizado
        .Build();                                // ← Constrói e retorna o token

    // 4. Usa o token
    return Result.Success(new LoginDto { Token = token });
}
```

**O que acontece internamente:**

```csharp
// Dentro do Builder:

// 1. Cada método WithXxx() armazena o valor
WithUserId(Guid userId)
{
    _userId = userId;  // ← Armazena em campo privado
    return this;       // ← Retorna this para encadeamento
}

// 2. Build() valida e constrói
Build()
{
    // Valida
    if (!_userId.HasValue) throw new Exception("...");
    
    // Constrói
    var claims = new List<Claim> { /* ... */ };
    var token = new JwtSecurityToken(/* ... */);
    
    // Retorna
    return tokenString;
}
```

---

## 💡 Benefícios

### 1. **Legibilidade**

```csharp
// ❌ ANTES: O que é cada parâmetro?
GenerateToken(id, username, email, role, minutes)

// ✅ DEPOIS: Auto-explicativo!
WithUserId(id).WithUsername(username).WithEmail(email)...
```

### 2. **Flexibilidade**

```csharp
// Fácil adicionar novos claims sem quebrar código existente
.WithCustomClaim("premium", "true")
.WithCustomClaim("subscription", "gold")
.WithCustomClaim("tier", "vip")
```

### 3. **Validação Centralizada**

```csharp
// Validações no Build(), não espalhadas pelo código
public string Build()
{
    if (!_userId.HasValue)
        throw new InvalidOperationException("UserId is required");
    // Todas as validações em um só lugar!
}
```

### 4. **Reutilização**

```csharp
// Mesmo builder pode ser usado em diferentes contextos
var adminToken = builder.WithRole("Admin").Build();
var userToken = builder.WithRole("User").Build();
```

### 5. **Testabilidade**

```csharp
// Fácil mockar o builder nos testes
var mockBuilder = new Mock<IJwtTokenBuilder>();
mockBuilder.Setup(b => b.WithUserId(It.IsAny<Guid>())).Returns(mockBuilder.Object);
```

### 6. **Extensibilidade**

```csharp
// Adicionar novos métodos não quebra código existente
public IJwtTokenBuilder WithIpAddress(string ip)
{
    _ipAddress = ip;
    return this;
}
// Código antigo continua funcionando!
```

---

## 📊 Comparação: Antes vs Depois

### Antes (Método Tradicional)

```csharp
// ❌ LoginCommand.cs
var token = _jwtTokenGenerator.GenerateToken(
    user.Id,                    // Parâmetro 1
    user.Username,             // Parâmetro 2
    user.Email,                // Parâmetro 3
    user.Role.ToString(),      // Parâmetro 4
    accessTokenExpirationMinutes // Parâmetro 5
);

// Problemas:
// - Difícil lembrar a ordem
// - Difícil adicionar claims customizados
// - Validações espalhadas
// - Código repetitivo
```

### Depois (Com Builder)

```csharp
// ✅ LoginCommand.cs
var token = _jwtTokenBuilder
    .WithUserId(user.Id)
    .WithUsername(user.Username)
    .WithEmail(user.Email)
    .WithRole(user.Role.ToString())
    .WithExpirationMinutes(accessTokenExpirationMinutes)
    .WithCustomClaim("premium", "true")  // Fácil adicionar!
    .Build();

// Vantagens:
// - Legível: cada linha explica o que faz
// - Flexível: fácil adicionar novos claims
// - Validação centralizada
// - Código mais limpo
```

### Comparação Visual

| Aspecto | Antes | Depois |
|---------|-------|--------|
| **Legibilidade** | ❌ Difícil entender parâmetros | ✅ Auto-explicativo |
| **Extensibilidade** | ❌ Quebra código existente | ✅ Não quebra código |
| **Validação** | ❌ Espalhada | ✅ Centralizada |
| **Testabilidade** | ⚠️ Difícil mockar | ✅ Fácil mockar |
| **Manutenibilidade** | ❌ Difícil manter | ✅ Fácil manter |

---

## 🎓 Quando Usar o Builder Pattern?

### ✅ Use quando:

1. **Objeto complexo com muitos parâmetros**
   - Mais de 4-5 parâmetros
   - Alguns parâmetros são opcionais

2. **Diferentes representações do mesmo objeto**
   - Tokens diferentes para diferentes contextos
   - Configurações variadas

3. **Validação complexa**
   - Múltiplas validações antes de construir
   - Validações que dependem de outros campos

4. **Código que precisa ser extensível**
   - Adicionar novos parâmetros sem quebrar código
   - Suportar diferentes tipos de construção

### ❌ Não use quando:

1. **Objeto simples com poucos parâmetros**
   - 1-3 parâmetros: construtor simples é suficiente

2. **Todos os parâmetros são obrigatórios**
   - Se não há opcionais, construtor tradicional é melhor

3. **Performance crítica**
   - Builder adiciona uma camada de abstração
   - Em casos extremos, pode ser overhead

---

## 🔄 Relação com Outros Patterns

### Builder vs Factory

- **Factory:** Cria objetos diferentes (diferentes tipos)
- **Builder:** Cria variações do mesmo objeto (mesmo tipo, configurações diferentes)

```csharp
// Factory: Cria tipos diferentes
var adminToken = factory.CreateAdminToken();
var userToken = factory.CreateUserToken();

// Builder: Cria variações do mesmo tipo
var token = builder.WithRole("Admin").Build();
var token2 = builder.WithRole("User").Build();
```

### Builder vs Strategy

- **Strategy:** Algoritmos diferentes para o mesmo problema
- **Builder:** Construção passo a passo do mesmo objeto

---

## 📚 Referências

- [Refactoring.Guru - Builder Pattern](https://refactoring.guru/design-patterns/builder)
- [Refactoring.Guru - Design Patterns Catalog](https://refactoring.guru/design-patterns)
- [Gang of Four - Design Patterns: Elements of Reusable Object-Oriented Software](https://en.wikipedia.org/wiki/Design_Patterns)

---

## ✅ Resumo

O **Builder Pattern** foi implementado para:

1. ✅ **Melhorar legibilidade** - Código auto-explicativo
2. ✅ **Facilitar extensão** - Adicionar novos claims sem quebrar código
3. ✅ **Centralizar validação** - Todas as validações no `Build()`
4. ✅ **Aumentar flexibilidade** - Construção passo a passo
5. ✅ **Melhorar testabilidade** - Fácil mockar o builder

**Onde foi aplicado:**
- `LoginCommand` - Geração de token no login
- `RefreshTokenCommand` - Geração de token no refresh

**Arquivos criados:**
- `src/GymDogs.Application/Interfaces/IJwtTokenBuilder.cs`
- `src/GymDogs.Infrastructure/Services/JwtTokenBuilder.cs`

**Arquivos modificados:**
- `src/GymDogs.Application/Users/Commands/LoginCommand.cs`
- `src/GymDogs.Application/Users/Commands/RefreshTokenCommand.cs`
- `src/GymDogs.Presentation/Program.cs` (registro no DI)

---

**🎉 Builder Pattern implementado com sucesso!**
