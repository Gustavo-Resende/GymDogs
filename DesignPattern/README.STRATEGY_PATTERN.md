# 🎯 Strategy Pattern - Explicação Didática

## 📚 O que é o Strategy Pattern?

O **Strategy Pattern** (Padrão de Estratégia) é um padrão de design comportamental que permite definir uma família de algoritmos, encapsulá-los e torná-los intercambiáveis. Ele permite que o algoritmo varie independentemente dos clientes que o utilizam.

---

## 🌍 Analogia do Mundo Real: Sistema de Pagamento

Imagine que você está em uma loja e precisa pagar uma compra. A loja aceita diferentes formas de pagamento:

- 💳 **Cartão de Crédito**
- 💵 **Dinheiro**
- 📱 **PIX**
- 🏦 **Boleto**

Cada forma de pagamento tem um **processo diferente**:
- Cartão: precisa passar na máquina, verificar saldo, etc.
- Dinheiro: precisa contar, dar troco, etc.
- PIX: precisa escanear QR code, confirmar, etc.
- Boleto: precisa gerar código, imprimir, etc.

**Mas todas elas têm o mesmo objetivo:** finalizar o pagamento.

No Strategy Pattern:
- **A interface** = "Forma de Pagamento" (todas têm o método `Pagar()`)
- **As estratégias** = Cartão, Dinheiro, PIX, Boleto (cada uma implementa `Pagar()` de forma diferente)
- **O contexto** = O caixa da loja (que escolhe qual estratégia usar)

---

## 🏗️ Como Funciona no Código?

### Estrutura do Pattern

```
┌─────────────────────────────────────┐
│   Context (ExceptionToResultMapper)  │
│   - Usa a estratégia                 │
│   - Não sabe como ela funciona      │
└──────────────┬──────────────────────┘
               │
               │ usa
               ▼
┌─────────────────────────────────────┐
│   IStrategy (IExceptionMappingStrategy)│
│   - Define o contrato                │
│   - Interface comum                  │
└──────────────┬──────────────────────┘
               │
               │ implementam
               ├──────────────────────┐
               │                      │
               ▼                      ▼
    ┌──────────────────┐   ┌──────────────────┐
    │ Strategy A       │   │ Strategy B       │
    │ (ArgumentNull)   │   │ (ArgumentException)│
    └──────────────────┘   └──────────────────┘
```

---

## 📍 Onde Aplicamos no Projeto GymDogs?

### Problema Anterior

Antes, tínhamos um método gigante com um `switch` que tratava todos os tipos de exceção:

```csharp
// ❌ ANTES: Tudo em um lugar só
public Result<T> MapToResult<T>(Exception exception)
{
    return exception switch
    {
        ArgumentNullException nullEx => Result<T>.Invalid(...),
        ArgumentException argEx => Result<T>.Invalid(...),
        InvalidOperationException opEx => Result<T>.Error(...),
        _ => Result<T>.Error("An unexpected error occurred.")
    };
}
```

**Problemas:**
- ❌ Difícil adicionar novos tipos de exceção (precisa modificar o método)
- ❌ Difícil testar cada caso isoladamente
- ❌ Viola o princípio Open/Closed (aberto para extensão, fechado para modificação)
- ❌ Código difícil de manter

### Solução com Strategy Pattern

Agora, cada tipo de exceção tem sua própria "classe especialista":

```csharp
// ✅ DEPOIS: Cada estratégia em sua própria classe
public class ArgumentNullExceptionStrategy : IExceptionMappingStrategy
{
    public bool CanHandle(Exception exception) => exception is ArgumentNullException;
    
    public Result<T> MapToResult<T>(Exception exception)
    {
        // Lógica específica para ArgumentNullException
    }
}
```

**Benefícios:**
- ✅ Fácil adicionar novos tipos (criar nova classe)
- ✅ Fácil testar cada estratégia isoladamente
- ✅ Respeita Open/Closed Principle
- ✅ Código mais limpo e organizado

---

## 🔍 Parte por Parte: Como Implementamos

### 1️⃣ A Interface (Contrato)

**Arquivo:** `src/GymDogs.Application/Common/ExceptionMapping/IExceptionMappingStrategy.cs`

```csharp
public interface IExceptionMappingStrategy
{
    bool CanHandle(Exception exception);
    Result<T> MapToResult<T>(Exception exception) where T : class;
}
```

**Analogia:** É como o "contrato" que todas as formas de pagamento devem seguir. Todas precisam ter:
- Um método para verificar se podem processar (`CanHandle`)
- Um método para processar (`MapToResult`)

**Por quê?**
- Define o que todas as estratégias devem fazer
- Garante que todas seguem o mesmo padrão
- Permite que o contexto use qualquer estratégia sem saber qual é

---

### 2️⃣ As Estratégias Específicas

Cada estratégia é uma classe que implementa a interface e trata um tipo específico de exceção.

#### Estratégia 1: ArgumentNullExceptionStrategy

**Arquivo:** `src/GymDogs.Application/Common/ExceptionMapping/Strategies/ArgumentNullExceptionStrategy.cs`

```csharp
public class ArgumentNullExceptionStrategy : IExceptionMappingStrategy
{
    public bool CanHandle(Exception exception)
    {
        return exception is ArgumentNullException;
    }

    public Result<T> MapToResult<T>(Exception exception) where T : class
    {
        var argNullEx = (ArgumentNullException)exception;
        return Result<T>.Invalid(new List<ValidationError> { ... });
    }
}
```

**Analogia:** É como o "processador de cartão de crédito". Ele sabe:
- Quando pode processar: "Sim, é um cartão de crédito" (`CanHandle`)
- Como processar: passar na máquina, verificar saldo, etc. (`MapToResult`)

**O que faz:**
- Trata exceções quando um parâmetro obrigatório é `null`
- Converte para `Result.Invalid` (erro de validação)
- Retorna mensagem amigável para o usuário

---

#### Estratégia 2: ArgumentExceptionStrategy

**Arquivo:** `src/GymDogs.Application/Common/ExceptionMapping/Strategies/ArgumentExceptionStrategy.cs`

```csharp
public class ArgumentExceptionStrategy : IExceptionMappingStrategy
{
    public bool CanHandle(Exception exception)
    {
        return exception is ArgumentException && exception is not ArgumentNullException;
    }

    public Result<T> MapToResult<T>(Exception exception) where T : class
    {
        var argEx = (ArgumentException)exception;
        return Result<T>.Invalid(new List<ValidationError> { ... });
    }
}
```

**Analogia:** É como o "processador de dinheiro". Ele sabe:
- Quando pode processar: "Sim, é dinheiro" (mas não é cartão)
- Como processar: contar, dar troco, etc.

**O que faz:**
- Trata exceções quando um argumento é inválido (mas não nulo)
- Converte para `Result.Invalid`
- Retorna a mensagem da exceção original

---

#### Estratégia 3: InvalidOperationExceptionStrategy

**Arquivo:** `src/GymDogs.Application/Common/ExceptionMapping/Strategies/InvalidOperationExceptionStrategy.cs`

```csharp
public class InvalidOperationExceptionStrategy : IExceptionMappingStrategy
{
    public bool CanHandle(Exception exception)
    {
        return exception is InvalidOperationException;
    }

    public Result<T> MapToResult<T>(Exception exception) where T : class
    {
        var opEx = (InvalidOperationException)exception;
        return Result<T>.Error(opEx.Message);
    }
}
```

**Analogia:** É como o "processador de PIX". Ele sabe:
- Quando pode processar: "Sim, é PIX"
- Como processar: escanear QR code, confirmar, etc.

**O que faz:**
- Trata exceções quando uma operação não pode ser executada
- Converte para `Result.Error` (erro de sistema, não validação)
- Retorna a mensagem da exceção

---

#### Estratégia 4: DefaultExceptionStrategy (Fallback)

**Arquivo:** `src/GymDogs.Application/Common/ExceptionMapping/Strategies/DefaultExceptionStrategy.cs`

```csharp
public class DefaultExceptionStrategy : IExceptionMappingStrategy
{
    public bool CanHandle(Exception exception)
    {
        return true; // Sempre pode tratar (fallback)
    }

    public Result<T> MapToResult<T>(Exception exception) where T : class
    {
        return Result<T>.Error("An unexpected error occurred.");
    }
}
```

**Analogia:** É como o "caixa manual" quando nenhuma máquina funciona. Ele:
- Sempre pode processar (fallback)
- Processa de forma genérica

**O que faz:**
- Trata qualquer exceção não mapeada por outras estratégias
- Retorna mensagem genérica de erro
- Garante que nunca ficamos sem resposta

---

### 3️⃣ O Contexto (Quem Usa as Estratégias)

**Arquivo:** `src/GymDogs.Presentation/Services/ExceptionToResultMapper.cs`

```csharp
public class ExceptionToResultMapper : IExceptionToResultMapper
{
    private readonly IEnumerable<IExceptionMappingStrategy> _strategies;

    public ExceptionToResultMapper(IEnumerable<IExceptionMappingStrategy> strategies)
    {
        _strategies = strategies;
    }

    public Result<T> MapToResult<T>(Exception exception) where T : class
    {
        // Encontra a primeira estratégia que pode tratar esta exceção
        var strategy = _strategies.FirstOrDefault(s => s.CanHandle(exception));
        
        // Usa a estratégia encontrada
        return strategy?.MapToResult<T>(exception) 
               ?? Result<T>.Error("An unexpected error occurred.");
    }
}
```

**Analogia:** É como o "caixa da loja". Ele:
- Tem acesso a todas as formas de pagamento (recebe todas as estratégias)
- Quando alguém quer pagar, ele pergunta: "Qual forma de pagamento você quer usar?"
- Escolhe a estratégia certa baseado na escolha do cliente
- Delega o processamento para a estratégia escolhida

**O que faz:**
- Recebe todas as estratégias via Dependency Injection
- Quando recebe uma exceção, pergunta a cada estratégia: "Você pode tratar isso?"
- Usa a primeira estratégia que disser "sim"
- Delega o trabalho para a estratégia

---

### 4️⃣ Registro no Dependency Injection

**Arquivo:** `src/GymDogs.Presentation/Program.cs`

```csharp
// Registra todas as estratégias
builder.Services.AddScoped<IExceptionMappingStrategy, ArgumentNullExceptionStrategy>();
builder.Services.AddScoped<IExceptionMappingStrategy, ArgumentExceptionStrategy>();
builder.Services.AddScoped<IExceptionMappingStrategy, InvalidOperationExceptionStrategy>();
builder.Services.AddScoped<IExceptionMappingStrategy, DefaultExceptionStrategy>();

// Registra o contexto que usa as estratégias
builder.Services.AddScoped<IExceptionToResultMapper, ExceptionToResultMapper>();
```

**Analogia:** É como "contratar os funcionários" da loja:
- Você contrata um funcionário para cada forma de pagamento
- Você contrata o caixa que vai usar esses funcionários
- Quando alguém precisa pagar, o caixa escolhe o funcionário certo

**Por quê a ordem importa?**
- Estratégias específicas primeiro (ArgumentNull, Argument, InvalidOperation)
- Estratégia genérica por último (Default)
- Assim, quando o contexto pergunta "quem pode tratar?", as específicas respondem primeiro

---

## 🎯 Fluxo Completo: Como Funciona na Prática

### Cenário: Uma exceção `ArgumentNullException` é lançada

```
1. Exceção lançada
   └─> ArgumentNullException: "userId cannot be null"

2. ExceptionToResultMapper recebe a exceção
   └─> MapToResult<CreateUserDto>(exception)

3. Mapper pergunta a cada estratégia: "Você pode tratar isso?"
   ├─> ArgumentNullExceptionStrategy.CanHandle(exception)
   │   └─> ✅ Sim! (exception is ArgumentNullException)
   ├─> ArgumentExceptionStrategy.CanHandle(exception)
   │   └─> ❌ Não (não é ArgumentException genérico)
   ├─> InvalidOperationExceptionStrategy.CanHandle(exception)
   │   └─> ❌ Não (não é InvalidOperationException)
   └─> DefaultExceptionStrategy.CanHandle(exception)
       └─> ✅ Sim (sempre retorna true, mas não chega aqui)

4. Mapper escolhe a primeira que disse "sim"
   └─> ArgumentNullExceptionStrategy

5. Mapper delega o trabalho
   └─> ArgumentNullExceptionStrategy.MapToResult<CreateUserDto>(exception)

6. Estratégia processa e retorna
   └─> Result<CreateUserDto>.Invalid([ValidationError])

7. Mapper retorna o resultado
   └─> Result<CreateUserDto>.Invalid([ValidationError])
```

---

## ✅ Benefícios do Strategy Pattern

### 1. **Open/Closed Principle**
- ✅ **Aberto para extensão:** Adicione novas estratégias sem modificar código existente
- ✅ **Fechado para modificação:** Não precisa mexer no `ExceptionToResultMapper`

**Exemplo:** Quer adicionar tratamento para `UnauthorizedAccessException`?
- Antes: Modificar o método `MapToResult` (viola Open/Closed)
- Agora: Criar `UnauthorizedAccessExceptionStrategy` e registrar no DI

### 2. **Single Responsibility Principle**
- Cada estratégia tem uma única responsabilidade: tratar um tipo de exceção
- O contexto tem uma única responsabilidade: escolher a estratégia certa

### 3. **Testabilidade**
- Cada estratégia pode ser testada isoladamente
- O contexto pode ser testado com mocks das estratégias

### 4. **Manutenibilidade**
- Código mais organizado e fácil de entender
- Fácil encontrar onde cada tipo de exceção é tratado

### 5. **Flexibilidade**
- Pode trocar estratégias em runtime (se necessário)
- Pode desabilitar estratégias facilmente (remover do DI)

---

## 📊 Comparação: Antes vs. Depois

| Aspecto | ❌ Antes (Switch) | ✅ Depois (Strategy) |
|---------|-------------------|----------------------|
| **Adicionar novo tipo** | Modificar método existente | Criar nova classe |
| **Testar isoladamente** | Difícil (tudo junto) | Fácil (cada estratégia) |
| **Manutenibilidade** | Código grande e confuso | Código pequeno e claro |
| ** **Open/Closed** | ❌ Viola | ✅ Respeita |
| **Single Responsibility** | ❌ Viola | ✅ Respeita |

---

## 🧪 Como Testar

### Teste de uma Estratégia Específica

```csharp
[Fact]
public void ArgumentNullExceptionStrategy_ShouldHandleArgumentNullException()
{
    // Arrange
    var strategy = new ArgumentNullExceptionStrategy();
    var exception = new ArgumentNullException("userId");

    // Act
    var canHandle = strategy.CanHandle(exception);
    var result = strategy.MapToResult<CreateUserDto>(exception);

    // Assert
    Assert.True(canHandle);
    Assert.Equal(ResultStatus.Invalid, result.Status);
    Assert.Single(result.ValidationErrors);
}
```

### Teste do Contexto (Mapper)

```csharp
[Fact]
public void ExceptionToResultMapper_ShouldUseCorrectStrategy()
{
    // Arrange
    var strategies = new List<IExceptionMappingStrategy>
    {
        new ArgumentNullExceptionStrategy(),
        new DefaultExceptionStrategy()
    };
    var mapper = new ExceptionToResultMapper(strategies);
    var exception = new ArgumentNullException("userId");

    // Act
    var result = mapper.MapToResult<CreateUserDto>(exception);

    // Assert
    Assert.Equal(ResultStatus.Invalid, result.Status);
}
```

---

## 🎓 Resumo: O que Aprendemos?

1. **Strategy Pattern** = Família de algoritmos intercambiáveis
2. **Interface** = Contrato que todas as estratégias seguem
3. **Estratégias** = Implementações específicas de cada algoritmo
4. **Contexto** = Quem escolhe e usa a estratégia certa
5. **Benefícios** = Código mais limpo, testável e extensível

---

## 📁 Estrutura de Arquivos Criada

```
src/GymDogs.Application/
└── Common/
    └── ExceptionMapping/
        ├── IExceptionMappingStrategy.cs          (Interface)
        └── Strategies/
            ├── ArgumentNullExceptionStrategy.cs  (Estratégia 1)
            ├── ArgumentExceptionStrategy.cs      (Estratégia 2)
            ├── InvalidOperationExceptionStrategy.cs (Estratégia 3)
            └── DefaultExceptionStrategy.cs       (Estratégia 4 - Fallback)

src/GymDogs.Presentation/
└── Services/
    └── ExceptionToResultMapper.cs                (Contexto - Refatorado)
```

---

## 🚀 Próximos Passos

Agora que você entendeu o Strategy Pattern, podemos aplicar outros patterns:

1. **Factory Pattern** - Para criar Specifications
2. **Builder Pattern** - Para construir JWT Tokens

Quer que eu implemente algum deles agora? 🎯
