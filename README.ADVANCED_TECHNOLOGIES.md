# 🚀 Tecnologias Avançadas para Entrevista Técnica - LogComex

Este documento explica conceitos avançados de desenvolvimento .NET que são frequentemente abordados em entrevistas técnicas, especialmente em empresas como a **LogComex** (empresa de tecnologia para logística e comércio exterior). Cada conceito será explicado de forma didática e com exemplos práticos de como aplicá-los no sistema GymDogs.

---

## 📋 Índice

1. [Sobre a LogComex](#sobre-a-logcomex)
2. [Filas e Mensageria](#filas-e-mensageria)
3. [Paralelismo](#paralelismo)
4. [Circuit Breaker com Polly](#circuit-breaker-com-polly)
5. [Concorrência](#concorrência)
6. [APIs Externas](#apis-externas)
7. [Como Aplicar no GymDogs](#como-aplicar-no-gymdogs)
8. [Exemplos Práticos](#exemplos-práticos)

---

## 🏢 Sobre a LogComex

### Contexto da Empresa

A **LogComex** é uma empresa brasileira de tecnologia focada em soluções para **logística e comércio exterior**. Eles desenvolvem sistemas que:

- **Processam grandes volumes de dados** (documentos fiscais, notas fiscais, declarações)
- **Integram com múltiplas APIs externas** (Receita Federal, SEFAZ, portos, etc.)
- **Precisam de alta disponibilidade** (sistemas críticos para operações de importação/exportação)
- **Trabalham com processamento assíncrono** (processamento de documentos em background)
- **Exigem resiliência** (sistemas não podem falhar mesmo se APIs externas estiverem instáveis)

### Por que essas tecnologias são importantes?

Em um ambiente como o da LogComex, você precisa:

1. **Filas (Mensageria)**: Processar milhares de documentos fiscais sem sobrecarregar o sistema
2. **Paralelismo**: Processar múltiplos documentos simultaneamente
3. **Circuit Breaker**: Proteger o sistema quando APIs da Receita Federal estão instáveis
4. **Concorrência**: Garantir que múltiplos usuários possam trabalhar simultaneamente sem conflitos
5. **APIs Externas**: Integrar com sistemas governamentais e de logística

---

## 📨 Filas e Mensageria

### O que é?

**Mensageria** é um padrão de comunicação assíncrona onde mensagens são enviadas para uma **fila** e processadas por **consumidores** em background, sem bloquear o sistema principal.

### Analogia do Mundo Real

Imagine um **restaurante**:
- **Cliente faz pedido** → Coloca na **fila** (balcão)
- **Cozinheiro** pega pedidos da fila e prepara
- Cliente não precisa esperar na cozinha, pode fazer outras coisas
- Se houver muitos pedidos, eles ficam na fila esperando

**Mensageria funciona assim:**
- **Aplicação envia tarefa** → Coloca na **fila** (RabbitMQ, Azure Service Bus, etc.)
- **Worker processa** tarefas da fila em background
- Aplicação não fica bloqueada esperando
- Se houver muitas tarefas, elas ficam na fila

### Por que usar?

1. **Desacoplamento**: Produtor não precisa conhecer o consumidor
2. **Escalabilidade**: Pode ter múltiplos workers processando
3. **Resiliência**: Se um worker falhar, outro pega a tarefa
4. **Performance**: Não bloqueia a thread principal
5. **Garantia de entrega**: Mensagens não são perdidas

### Tecnologias Comuns

- **RabbitMQ**: Open source, muito popular
- **Azure Service Bus**: Serviço gerenciado da Microsoft
- **Amazon SQS**: Serviço da AWS
- **Redis**: Pode ser usado como fila simples
- **Apache Kafka**: Para streams de dados

### Exemplo Prático: Processar Upload de Documentos

**Sem Fila (Síncrono - Ruim):**
```csharp
// ❌ Bloqueia a thread até processar tudo
public async Task<IActionResult> UploadDocument(IFormFile file)
{
    // Processa documento (pode levar 30 segundos)
    var result = await ProcessDocument(file);
    return Ok(result);
}
```

**Com Fila (Assíncrono - Bom):**
```csharp
// ✅ Retorna imediatamente, processa em background
public async Task<IActionResult> UploadDocument(IFormFile file)
{
    // Envia para fila (retorna em < 1 segundo)
    await _messageQueue.PublishAsync(new ProcessDocumentMessage 
    { 
        FileId = fileId,
        FileContent = fileBytes 
    });
    
    return Accepted(new { messageId = messageId });
}
```

---

## ⚡ Paralelismo

### O que é?

**Paralelismo** é a execução de múltiplas tarefas **simultaneamente**, aproveitando múltiplos núcleos do processador.

### Diferença: Paralelismo vs Concorrência

| Conceito | Definição | Exemplo |
|----------|-----------|---------|
| **Concorrência** | Múltiplas tarefas **alternando** execução (uma por vez) | 1 cozinheiro fazendo 3 pratos (alterna entre eles) |
| **Paralelismo** | Múltiplas tarefas **executando ao mesmo tempo** | 3 cozinheiros, cada um fazendo 1 prato simultaneamente |

### Quando usar?

✅ **Use Paralelismo quando:**
- Processar **muitos itens independentes** (ex: 1000 exercícios)
- Operações **CPU-intensive** (cálculos, transformações)
- Tem **múltiplos núcleos** disponíveis

❌ **Não use quando:**
- Operações são **I/O-bound** (esperar banco de dados, APIs)
- Itens têm **dependências** entre si
- Poucos itens para processar (overhead não compensa)

### Exemplos em C#

#### 1. Task.Run (Simples)

```csharp
// Executa tarefa em thread separada
var task = Task.Run(() => 
{
    // Processamento pesado
    ProcessLargeDataSet(data);
});

// Continua executando outras coisas
await DoOtherWork();

// Aguarda resultado quando necessário
await task;
```

#### 2. Parallel.ForEach (Processar Lista)

```csharp
// Processa múltiplos itens em paralelo
var exercises = await GetAllExercises();

Parallel.ForEach(exercises, exercise =>
{
    // Cada exercício processado em thread separada
    ProcessExercise(exercise);
});
```

#### 3. Task.WhenAll (Múltiplas Tarefas)

```csharp
// Executa múltiplas tarefas em paralelo
var task1 = ProcessExercisesAsync();
var task2 = ProcessProfilesAsync();
var task3 = ProcessWorkoutsAsync();

// Aguarda todas completarem
await Task.WhenAll(task1, task2, task3);
```

### Exemplo Prático: Processar Múltiplos Exercícios

```csharp
// ❌ Sequencial (lento)
public async Task ProcessExercises(IEnumerable<Exercise> exercises)
{
    foreach (var exercise in exercises)
    {
        await ProcessExercise(exercise); // 1 segundo cada = 60 segundos total
    }
}

// ✅ Paralelo (rápido)
public async Task ProcessExercises(IEnumerable<Exercise> exercises)
{
    var tasks = exercises.Select(exercise => 
        Task.Run(async () => await ProcessExercise(exercise))
    );
    
    await Task.WhenAll(tasks); // Todos processam simultaneamente = ~1 segundo
}
```

---

## 🔌 Circuit Breaker com Polly

### O que é?

**Circuit Breaker** é um padrão que **protege o sistema** quando um serviço externo está falhando. Funciona como um **disjuntor elétrico**:

1. **Normal**: Circuito fechado, requisições passam
2. **Falhas**: Após N falhas, circuito abre
3. **Aberto**: Requisições são bloqueadas imediatamente (sem tentar)
4. **Recuperação**: Após X segundos, tenta novamente (half-open)
5. **Sucesso**: Volta ao normal (fechado)

### Por que usar?

- **Evita sobrecarga**: Não fica tentando chamar API que está down
- **Resposta rápida**: Retorna erro imediatamente quando circuito está aberto
- **Auto-recuperação**: Tenta novamente automaticamente após um tempo
- **Protege recursos**: Não desperdiça threads esperando timeouts

### Biblioteca: Polly

**Polly** é a biblioteca mais popular para resiliência em .NET:

```bash
dotnet add package Polly
dotnet add package Microsoft.Extensions.Http.Polly
```

### Exemplo Básico

```csharp
using Polly;
using Polly.CircuitBreaker;

// Configurar Circuit Breaker
var circuitBreaker = Policy
    .Handle<HttpRequestException>()
    .CircuitBreakerAsync(
        handledEventsAllowedBeforeBreaking: 3,  // Abre após 3 falhas
        durationOfBreak: TimeSpan.FromSeconds(30) // Fica aberto por 30s
    );

// Usar
try
{
    await circuitBreaker.ExecuteAsync(async () =>
    {
        return await httpClient.GetAsync("https://api.externa.com/data");
    });
}
catch (BrokenCircuitException)
{
    // Circuito está aberto - API está down
    // Retornar resposta em cache ou erro amigável
}
```

### Estados do Circuit Breaker

```
┌─────────────┐
│   CLOSED    │ ← Normal: requisições passam
│  (Fechado)  │
└──────┬──────┘
       │ 3 falhas
       ▼
┌─────────────┐
│    OPEN     │ ← Bloqueado: requisições falham imediatamente
│  (Aberto)   │
└──────┬──────┘
       │ 30 segundos
       ▼
┌─────────────┐
│  HALF-OPEN  │ ← Testando: permite 1 requisição de teste
│ (Meio-Aberto)│
└──────┬──────┘
       │ Sucesso? → CLOSED
       │ Falha?   → OPEN
```

### Exemplo Prático: Integração com API Externa

```csharp
public class ExternalApiService
{
    private readonly HttpClient _httpClient;
    private readonly IAsyncPolicy<HttpResponseMessage> _circuitBreaker;

    public ExternalApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        
        // Configurar Circuit Breaker
        _circuitBreaker = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .Or<HttpRequestException>()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(60)
            );
    }

    public async Task<string> GetDataAsync()
    {
        try
        {
            var response = await _circuitBreaker.ExecuteAsync(async () =>
            {
                return await _httpClient.GetAsync("/api/data");
            });
            
            return await response.Content.ReadAsStringAsync();
        }
        catch (BrokenCircuitException)
        {
            // API está down - retornar cache ou erro amigável
            return GetCachedData() ?? throw new ServiceUnavailableException();
        }
    }
}
```

---

## 🔄 Concorrência

### O que é?

**Concorrência** é quando múltiplas operações **compartilham recursos** e precisam ser **coordenadas** para evitar conflitos (race conditions).

### Problema: Race Condition

```csharp
// ❌ PROBLEMA: Race Condition
public class ExerciseService
{
    private int _viewCount = 0;

    public void IncrementViews()
    {
        _viewCount++; // Não é thread-safe!
        // Thread 1: lê 100, incrementa para 101
        // Thread 2: lê 100 (antes de Thread 1 salvar), incrementa para 101
        // Resultado: deveria ser 102, mas é 101
    }
}
```

### Soluções

#### 1. Lock (Bloqueio)

```csharp
private readonly object _lock = new object();
private int _viewCount = 0;

public void IncrementViews()
{
    lock (_lock) // Apenas 1 thread por vez
    {
        _viewCount++;
    }
}
```

#### 2. SemaphoreSlim (Limitar Concorrência)

```csharp
private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(5); // Máximo 5 simultâneos

public async Task ProcessAsync()
{
    await _semaphore.WaitAsync(); // Aguarda se já tem 5 executando
    try
    {
        await DoWork();
    }
    finally
    {
        _semaphore.Release(); // Libera para próximo
    }
}
```

#### 3. Concurrent Collections

```csharp
// Thread-safe collections
var concurrentBag = new ConcurrentBag<Exercise>();
var concurrentDictionary = new ConcurrentDictionary<string, Exercise>();

// Múltiplas threads podem adicionar simultaneamente sem problemas
concurrentBag.Add(exercise);
```

#### 4. Interlocked (Operações Atômicas)

```csharp
private int _viewCount = 0;

public void IncrementViews()
{
    Interlocked.Increment(ref _viewCount); // Operação atômica
}
```

### Exemplo Prático: Contador de Visualizações

```csharp
public class ExerciseViewCounter
{
    private readonly ConcurrentDictionary<Guid, int> _viewCounts = new();
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(10); // Máx 10 simultâneos

    public async Task IncrementViewsAsync(Guid exerciseId)
    {
        await _semaphore.WaitAsync();
        try
        {
            _viewCounts.AddOrUpdate(
                exerciseId,
                key => 1,           // Se não existe, cria com valor 1
                (key, oldValue) => oldValue + 1 // Se existe, incrementa
            );
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
```

---

## 🌐 APIs Externas

### O que é?

**APIs Externas** são serviços de terceiros que seu sistema precisa chamar para obter dados ou executar ações (ex: Receita Federal, SEFAZ, serviços de pagamento, etc.).

### Desafios

1. **Latência**: Pode demorar para responder
2. **Instabilidade**: Pode estar down ou lento
3. **Rate Limiting**: Pode limitar número de requisições
4. **Autenticação**: Precisa de tokens, chaves, etc.
5. **Versionamento**: API pode mudar

### Boas Práticas

#### 1. HttpClient com Factory

```csharp
// ❌ Ruim: Criar HttpClient manualmente
var client = new HttpClient(); // Pode esgotar conexões

// ✅ Bom: Usar IHttpClientFactory
builder.Services.AddHttpClient<ExternalApiService>(client =>
{
    client.BaseAddress = new Uri("https://api.externa.com");
    client.Timeout = TimeSpan.FromSeconds(30);
});
```

#### 2. Retry Policy

```csharp
using Polly;
using Polly.Extensions.Http;

// Configurar retry
var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(
        retryCount: 3,
        sleepDurationProvider: retryAttempt => 
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) // 2s, 4s, 8s
    );
```

#### 3. Timeout

```csharp
var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(
    TimeSpan.FromSeconds(10) // Timeout após 10 segundos
);
```

#### 4. Fallback (Resposta Alternativa)

```csharp
var fallbackPolicy = Policy<string>
    .Handle<HttpRequestException>()
    .FallbackAsync("Dados em cache ou valor padrão");
```

### Exemplo Completo: Serviço de API Externa

```csharp
public class ExternalApiService
{
    private readonly HttpClient _httpClient;
    private readonly IAsyncPolicy<HttpResponseMessage> _resiliencePolicy;

    public ExternalApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ExternalApi");
        
        // Combinar múltiplas políticas
        _resiliencePolicy = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .Or<HttpRequestException>()
            .WaitAndRetryAsync(3, retryAttempt => 
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
            .WrapAsync(
                Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10))
            )
            .WrapAsync(
                Policy<HttpResponseMessage>
                    .Handle<HttpRequestException>()
                    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(60))
            );
    }

    public async Task<string> GetDataAsync()
    {
        try
        {
            var response = await _resiliencePolicy.ExecuteAsync(async () =>
            {
                return await _httpClient.GetAsync("/api/data");
            });
            
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (BrokenCircuitException)
        {
            // API está down - usar cache
            return GetCachedData();
        }
        catch (TimeoutRejectedException)
        {
            // Timeout - usar cache
            return GetCachedData();
        }
    }
}
```

---

## 🎯 Como Aplicar no GymDogs

### Cenário 1: Processar Upload de Exercícios em Lote

**Problema**: Admin quer fazer upload de 100 exercícios de uma vez. Processar síncrono bloquearia a API.

**Solução: Fila + Paralelismo**

```csharp
// 1. Controller recebe upload
[HttpPost("exercises/bulk")]
public async Task<IActionResult> BulkUploadExercises(IFormFile file)
{
    var fileId = Guid.NewGuid();
    
    // Envia para fila (retorna imediatamente)
    await _messageQueue.PublishAsync(new ProcessBulkExercisesMessage
    {
        FileId = fileId,
        UserId = GetCurrentUserId()
    });
    
    return Accepted(new { fileId, status = "processing" });
}

// 2. Worker processa em background (com paralelismo)
public class BulkExercisesProcessor
{
    public async Task ProcessAsync(ProcessBulkExercisesMessage message)
    {
        var exercises = await LoadExercisesFromFile(message.FileId);
        
        // Processa em paralelo (máximo 10 simultâneos)
        var semaphore = new SemaphoreSlim(10);
        var tasks = exercises.Select(async exercise =>
        {
            await semaphore.WaitAsync();
            try
            {
                await CreateExerciseAsync(exercise);
            }
            finally
            {
                semaphore.Release();
            }
        });
        
        await Task.WhenAll(tasks);
    }
}
```

### Cenário 2: Integrar com API de Nutrição Externa

**Problema**: Queremos buscar informações nutricionais de exercícios de uma API externa, mas ela pode estar instável.

**Solução: Circuit Breaker + Retry**

```csharp
public class NutritionApiService
{
    private readonly HttpClient _httpClient;
    private readonly IAsyncPolicy<HttpResponseMessage> _policy;

    public NutritionApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("NutritionApi");
        
        _policy = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .Or<HttpRequestException>()
            .WaitAndRetryAsync(3, retryAttempt => 
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
            .WrapAsync(
                Policy<HttpResponseMessage>
                    .Handle<HttpRequestException>()
                    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(60))
            );
    }

    public async Task<NutritionInfo> GetNutritionInfoAsync(string exerciseName)
    {
        try
        {
            var response = await _policy.ExecuteAsync(async () =>
            {
                return await _httpClient.GetAsync($"/api/nutrition/{exerciseName}");
            });
            
            return await response.Content.ReadFromJsonAsync<NutritionInfo>();
        }
        catch (BrokenCircuitException)
        {
            // API está down - retornar dados em cache ou null
            return GetCachedNutritionInfo(exerciseName);
        }
    }
}
```

### Cenário 3: Contador de Visualizações com Concorrência

**Problema**: Múltiplos usuários visualizando o mesmo exercício simultaneamente pode causar race condition.

**Solução: ConcurrentDictionary + Interlocked**

```csharp
public class ExerciseViewService
{
    private readonly ConcurrentDictionary<Guid, int> _viewCounts = new();
    private readonly IRepository<Exercise> _exerciseRepository;

    public async Task IncrementViewCountAsync(Guid exerciseId)
    {
        // Incrementa contador em memória (thread-safe)
        _viewCounts.AddOrUpdate(
            exerciseId,
            key => 1,
            (key, oldValue) => Interlocked.Increment(ref oldValue)
        );
        
        // Salva no banco periodicamente (em background)
        _ = Task.Run(async () =>
        {
            await Task.Delay(5000); // Aguarda 5 segundos
            await SaveViewCountToDatabaseAsync(exerciseId);
        });
    }
    
    private async Task SaveViewCountToDatabaseAsync(Guid exerciseId)
    {
        if (_viewCounts.TryRemove(exerciseId, out var count))
        {
            var exercise = await _exerciseRepository.GetByIdAsync(exerciseId);
            exercise.IncrementViews(count);
            await _exerciseRepository.UpdateAsync(exercise);
        }
    }
}
```

### Cenário 4: Buscar Exercícios de Múltiplas Fontes

**Problema**: Queremos buscar exercícios de nossa API + API externa e combinar resultados.

**Solução: Paralelismo com Task.WhenAll**

```csharp
public class ExerciseSearchService
{
    private readonly IRepository<Exercise> _exerciseRepository;
    private readonly ExternalExerciseApiService _externalApi;

    public async Task<IEnumerable<Exercise>> SearchExercisesAsync(string searchTerm)
    {
        // Busca em paralelo: nossa API + API externa
        var internalTask = _exerciseRepository.ListAsync(
            new SearchExercisesByNameSpec(searchTerm)
        );
        
        var externalTask = _externalApi.SearchExercisesAsync(searchTerm);
        
        // Aguarda ambas completarem
        await Task.WhenAll(internalTask, externalTask);
        
        var internalExercises = await internalTask;
        var externalExercises = await externalTask;
        
        // Combina resultados
        return internalExercises
            .Concat(externalExercises)
            .DistinctBy(e => e.Name)
            .OrderBy(e => e.Name);
    }
}
```

---

## 📚 Exemplos Práticos Completos

### Exemplo 1: Sistema de Notificações com Fila

```csharp
// 1. Interface da Fila
public interface IMessageQueue
{
    Task PublishAsync<T>(T message) where T : class;
    Task<T> ConsumeAsync<T>(CancellationToken cancellationToken = default) where T : class;
}

// 2. Enviar Notificação
[HttpPost("exercises/{id}/notify")]
public async Task<IActionResult> NotifyExerciseCreated(Guid id)
{
    await _messageQueue.PublishAsync(new ExerciseCreatedNotification
    {
        ExerciseId = id,
        Timestamp = DateTime.UtcNow
    });
    
    return Ok();
}

// 3. Worker Processa Notificações
public class NotificationWorker : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var notification = await _messageQueue.ConsumeAsync<ExerciseCreatedNotification>(stoppingToken);
            await ProcessNotificationAsync(notification);
        }
    }
}
```

### Exemplo 2: Processamento Paralelo de Estatísticas

```csharp
public class StatisticsService
{
    public async Task<StatisticsDto> CalculateStatisticsAsync()
    {
        // Calcula múltiplas estatísticas em paralelo
        var totalExercisesTask = GetTotalExercisesAsync();
        var totalUsersTask = GetTotalUsersAsync();
        var totalWorkoutsTask = GetTotalWorkoutsAsync();
        var popularExercisesTask = GetPopularExercisesAsync();
        
        await Task.WhenAll(
            totalExercisesTask,
            totalUsersTask,
            totalWorkoutsTask,
            popularExercisesTask
        );
        
        return new StatisticsDto
        {
            TotalExercises = await totalExercisesTask,
            TotalUsers = await totalUsersTask,
            TotalWorkouts = await totalWorkoutsTask,
            PopularExercises = await popularExercisesTask
        };
    }
}
```

### Exemplo 3: Integração Resiliente com API Externa

```csharp
public class ExternalExerciseApiService
{
    private readonly HttpClient _httpClient;
    private readonly IAsyncPolicy<HttpResponseMessage> _resiliencePolicy;
    private readonly IMemoryCache _cache;

    public ExternalExerciseApiService(
        IHttpClientFactory httpClientFactory,
        IMemoryCache cache)
    {
        _httpClient = httpClientFactory.CreateClient("ExternalExerciseApi");
        _cache = cache;
        
        _resiliencePolicy = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .Or<HttpRequestException>()
            .WaitAndRetryAsync(
                3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
            )
            .WrapAsync(
                Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10))
            )
            .WrapAsync(
                Policy<HttpResponseMessage>
                    .Handle<HttpRequestException>()
                    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(60))
            );
    }

    public async Task<IEnumerable<Exercise>> GetExercisesAsync()
    {
        // Tenta cache primeiro
        if (_cache.TryGetValue("external_exercises", out IEnumerable<Exercise> cached))
        {
            return cached;
        }
        
        try
        {
            var response = await _resiliencePolicy.ExecuteAsync(async () =>
            {
                return await _httpClient.GetAsync("/api/exercises");
            });
            
            var exercises = await response.Content.ReadFromJsonAsync<IEnumerable<Exercise>>();
            
            // Cache por 5 minutos
            _cache.Set("external_exercises", exercises, TimeSpan.FromMinutes(5));
            
            return exercises;
        }
        catch (BrokenCircuitException)
        {
            // API está down - retornar cache mesmo que expirado
            return cached ?? Enumerable.Empty<Exercise>();
        }
        catch (TimeoutRejectedException)
        {
            // Timeout - retornar cache
            return cached ?? Enumerable.Empty<Exercise>();
        }
    }
}
```

---

## 🎓 Resumo para Entrevista

### Filas (Mensageria)

**O que dizer:**
> "Filas permitem processamento assíncrono. Quando preciso processar algo pesado (ex: upload de arquivos), envio para uma fila e retorno imediatamente. Um worker processa em background. Isso melhora performance e escalabilidade."

**Exemplo prático:**
> "No GymDogs, quando um admin faz upload de 100 exercícios, envio para uma fila RabbitMQ. A API retorna em < 1 segundo, e um worker processa os exercícios em background."

### Paralelismo

**O que dizer:**
> "Paralelismo executa múltiplas tarefas simultaneamente usando múltiplos núcleos. Uso `Task.WhenAll` ou `Parallel.ForEach` quando preciso processar muitos itens independentes."

**Exemplo prático:**
> "Para calcular estatísticas, busco dados de exercícios, usuários e treinos em paralelo com `Task.WhenAll`, reduzindo tempo de 3 segundos para 1 segundo."

### Circuit Breaker

**O que dizer:**
> "Circuit Breaker protege o sistema quando APIs externas estão falhando. Após N falhas, o circuito abre e bloqueia requisições imediatamente. Após X segundos, tenta novamente. Uso Polly para implementar."

**Exemplo prático:**
> "Se a API de nutrição externa falhar 5 vezes, o Circuit Breaker abre. Próximas requisições retornam erro imediatamente (sem tentar). Após 60 segundos, tenta novamente automaticamente."

### Concorrência

**O que dizer:**
> "Concorrência coordena acesso a recursos compartilhados. Uso `lock`, `SemaphoreSlim`, ou `ConcurrentDictionary` para evitar race conditions quando múltiplas threads acessam o mesmo recurso."

**Exemplo prático:**
> "Para contar visualizações de exercícios, uso `ConcurrentDictionary` com `Interlocked.Increment` para garantir que múltiplos usuários visualizando simultaneamente não causem perda de contagem."

### APIs Externas

**O que dizer:**
> "Para APIs externas, uso `IHttpClientFactory`, Polly para retry e circuit breaker, timeouts, e cache. Isso garante resiliência mesmo quando APIs estão instáveis."

**Exemplo prático:**
> "Ao buscar exercícios de API externa, configuro retry (3 tentativas), circuit breaker (abre após 5 falhas), timeout (10s), e cache (5 minutos). Se API estiver down, retorno cache."

---

## 📦 Pacotes NuGet Recomendados

```xml
<!-- Mensageria -->
<PackageReference Include="RabbitMQ.Client" Version="6.8.1" />
<PackageReference Include="MassTransit" Version="8.1.3" />

<!-- Resiliência (Polly) -->
<PackageReference Include="Polly" Version="8.2.0" />
<PackageReference Include="Microsoft.Extensions.Http.Polly" Version="8.0.0" />

<!-- HTTP Client -->
<PackageReference Include="Microsoft.Extensions.Http" Version="8.0.0" />
```

---

## ✅ Checklist para Entrevista

- [ ] Entender o que é mensageria e quando usar
- [ ] Saber diferença entre paralelismo e concorrência
- [ ] Conhecer Circuit Breaker e seus estados
- [ ] Saber usar Polly para resiliência
- [ ] Entender race conditions e como evitar
- [ ] Conhecer boas práticas para APIs externas
- [ ] Ter exemplos práticos prontos
- [ ] Saber explicar benefícios de cada tecnologia

---

**Última atualização:** Janeiro 2024  
**Versão:** 1.0.0
