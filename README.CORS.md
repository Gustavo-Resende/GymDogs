# 🌐 CORS - Cross-Origin Resource Sharing

## 📚 O que é CORS?

**CORS (Cross-Origin Resource Sharing)** é um mecanismo de segurança implementado pelos navegadores que controla quais requisições HTTP podem ser feitas de um domínio (origem) para outro.

### 🎯 Analogia do Mundo Real

Imagine que você está em uma **loja física** (seu front-end) e quer comprar algo de uma **loja online** (sua API):

- **Sem CORS:** A loja física não permite que você compre de outras lojas. O navegador bloqueia a requisição.
- **Com CORS:** A loja online diz: "Sim, você pode comprar aqui!" e lista quais lojas físicas são permitidas.

### 🔍 O que é "Origem"?

**Origem = Protocolo + Domínio + Porta**

Exemplos de origens diferentes:
- `http://localhost:3000` (React padrão)
- `http://localhost:5173` (Vite padrão)
- `http://localhost:8080` (Sua API)
- `https://meusite.com` (Produção)

**Mesma origem:**
- `http://localhost:8080/api/users` e `http://localhost:8080/api/profiles` ✅ (mesma origem)

**Origens diferentes:**
- `http://localhost:3000` (front-end) e `http://localhost:8080` (API) ❌ (origens diferentes)

---

## 🚫 Por que o CORS existe?

### Problema de Segurança

Sem CORS, qualquer site malicioso poderia fazer requisições para sua API em nome do usuário:

```javascript
// Site malicioso (evil.com) tentando acessar sua API
fetch('http://localhost:8080/api/users/me')
  .then(data => {
    // Roubar dados do usuário!
  });
```

### Solução: CORS

O navegador **bloqueia** requisições entre origens diferentes, **a menos que** o servidor (sua API) explicitamente permita.

---

## 🔧 Como Funciona o CORS?

### Fluxo de uma Requisição CORS

```
1. Front-end (localhost:3000) faz requisição para API (localhost:8080)
   ↓
2. Navegador detecta: "Origens diferentes!"
   ↓
3. Navegador envia requisição OPTIONS (preflight) para verificar permissões
   ↓
4. API responde com headers CORS:
   - Access-Control-Allow-Origin: http://localhost:3000
   - Access-Control-Allow-Methods: GET, POST, PUT, DELETE
   - Access-Control-Allow-Headers: Content-Type, Authorization
   ↓
5. Navegador verifica: "OK, a API permite essa origem!"
   ↓
6. Navegador envia a requisição real (GET, POST, etc.)
   ↓
7. API processa e responde normalmente
```

### Tipos de Requisições CORS

#### 1. **Simple Request** (Requisição Simples)

Requisições que **não precisam** de preflight:

- Métodos: GET, HEAD, POST
- Headers simples: Content-Type (apenas text/plain, application/x-www-form-urlencoded, multipart/form-data)
- Sem headers customizados

```javascript
// Exemplo de Simple Request
fetch('http://localhost:8080/api/profiles/public')
  .then(res => res.json());
```

#### 2. **Preflight Request** (Requisição com Pré-voo)

Requisições que **precisam** de verificação prévia:

- Métodos: PUT, DELETE, PATCH
- Headers customizados: Authorization, Content-Type: application/json
- Credentials (cookies, tokens)

```javascript
// Exemplo de Preflight Request
fetch('http://localhost:8080/api/profiles', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': 'Bearer token123'
  },
  body: JSON.stringify({ displayName: 'João' })
});
```

**O que acontece:**
1. Navegador envia `OPTIONS` primeiro
2. API responde com permissões CORS
3. Se permitido, navegador envia a requisição real

---

## ⚙️ Configuração no ASP.NET Core

### 1. Adicionar CORS no `Program.cs`

```csharp
// 1. Registrar serviços CORS (no builder)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()      // Permite qualquer header
              .AllowAnyMethod()      // Permite GET, POST, PUT, DELETE, etc.
              .AllowCredentials();   // Permite cookies e tokens
    });
});

// 2. Usar o middleware CORS (no app)
app.UseCors("AllowFrontend");
```

### 2. Ordem Importante dos Middlewares

A ordem dos middlewares **importa muito**! CORS deve vir **antes** de Authentication:

```csharp
var app = builder.Build();

// ✅ ORDEM CORRETA:
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");  // ← CORS primeiro!
app.UseAuthentication();       // ← Depois autenticação
app.UseAuthorization();         // ← Depois autorização
app.MapControllers();
```

**Por quê?**
- CORS precisa processar requisições OPTIONS (preflight) **antes** de autenticação
- Se autenticação rodar primeiro, pode bloquear requisições OPTIONS

### 3. Configuração por Ambiente

#### Desenvolvimento (`appsettings.Development.json`)

```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:5173",
      "http://localhost:5174"
    ]
  }
}
```

#### Produção (`appsettings.Production.json`)

```json
{
  "Cors": {
    "AllowedOrigins": [
      "https://meusite.com",
      "https://www.meusite.com"
    ]
  }
}
```

### 4. Código Dinâmico (Lendo do appsettings)

```csharp
// Ler origens do appsettings
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
```

---

## 🎯 Configurações Comuns

### Configuração 1: Desenvolvimento (Permissiva)

```csharp
// ⚠️ APENAS PARA DESENVOLVIMENTO!
builder.Services.AddCors(options =>
{
    options.AddPolicy("Development", policy =>
    {
        policy.AllowAnyOrigin()      // Qualquer origem
              .AllowAnyHeader()       // Qualquer header
              .AllowAnyMethod();      // Qualquer método
        // ⚠️ Não pode usar AllowCredentials() com AllowAnyOrigin()
    });
});
```

**Quando usar:** Apenas em desenvolvimento local, nunca em produção!

### Configuração 2: Produção (Restritiva)

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins("https://meusite.com", "https://www.meusite.com")
              .WithMethods("GET", "POST", "PUT", "DELETE", "PATCH")
              .WithHeaders("Content-Type", "Authorization")
              .AllowCredentials();
    });
});
```

**Quando usar:** Em produção, com origens específicas e métodos/headers restritos.

### Configuração 3: Por Ambiente

```csharp
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
        {
            policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });
}
else
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
        {
            policy.WithOrigins("https://meusite.com")
                  .WithMethods("GET", "POST", "PUT", "DELETE")
                  .WithHeaders("Content-Type", "Authorization")
                  .AllowCredentials();
        });
    });
}
```

---

## 🔍 Headers CORS Explicados

### Headers que a API Envia

| Header | Descrição | Exemplo |
|--------|-----------|---------|
| `Access-Control-Allow-Origin` | Quais origens podem acessar | `http://localhost:3000` |
| `Access-Control-Allow-Methods` | Quais métodos HTTP são permitidos | `GET, POST, PUT, DELETE` |
| `Access-Control-Allow-Headers` | Quais headers podem ser enviados | `Content-Type, Authorization` |
| `Access-Control-Allow-Credentials` | Permite cookies/tokens | `true` |
| `Access-Control-Max-Age` | Cache do preflight (segundos) | `3600` |

### Headers que o Front-end Envia

| Header | Descrição | Quando Usar |
|--------|-----------|-------------|
| `Origin` | Origem da requisição | Automático pelo navegador |
| `Access-Control-Request-Method` | Método que será usado | Em preflight (OPTIONS) |
| `Access-Control-Request-Headers` | Headers que serão usados | Em preflight (OPTIONS) |

---

## 🐛 Problemas Comuns e Soluções

### Problema 1: "CORS policy blocked"

**Erro no console:**
```
Access to fetch at 'http://localhost:8080/api/users' from origin 'http://localhost:3000' 
has been blocked by CORS policy
```

**Solução:**
1. Verificar se CORS está configurado no `Program.cs`
2. Verificar se a origem do front-end está na lista de permitidas
3. Verificar se `UseCors()` está antes de `UseAuthentication()`

### Problema 2: "Credentials not allowed"

**Erro:**
```
The value of the 'Access-Control-Allow-Credentials' header in the response is '' 
which must be 'true' when the request's credentials mode is 'include'.
```

**Causa:** Você está usando `AllowCredentials()` mas também `AllowAnyOrigin()`.

**Solução:**
```csharp
// ❌ ERRADO (não funciona):
policy.AllowAnyOrigin().AllowCredentials();

// ✅ CORRETO:
policy.WithOrigins("http://localhost:3000").AllowCredentials();
```

### Problema 3: Preflight falha (OPTIONS 405)

**Erro:** Requisição OPTIONS retorna 405 (Method Not Allowed)

**Causa:** Middleware CORS não está processando requisições OPTIONS.

**Solução:**
```csharp
// Garantir que CORS está antes de Authentication
app.UseCors("AllowFrontend");
app.UseAuthentication();
```

### Problema 4: CORS funciona no Postman mas não no navegador

**Causa:** Postman não aplica CORS (é uma ferramenta de servidor). Navegadores aplicam.

**Solução:** CORS está funcionando! O problema é que você precisa configurar no código.

---

## ✅ Checklist de Configuração

- [ ] CORS configurado no `Program.cs`
- [ ] Origens permitidas listadas corretamente
- [ ] `UseCors()` antes de `UseAuthentication()`
- [ ] `AllowCredentials()` se usar tokens/cookies
- [ ] Configuração diferente para Dev e Production
- [ ] Testado no navegador (não apenas Postman)

---

## 🧪 Como Testar

### 1. Teste no Navegador (Console do DevTools)

```javascript
// No console do navegador (F12)
fetch('http://localhost:8080/api/profiles/public')
  .then(res => res.json())
  .then(data => console.log(data))
  .catch(err => console.error('CORS Error:', err));
```

**Se funcionar:** Você verá os dados no console.  
**Se não funcionar:** Você verá erro de CORS no console.

### 2. Teste com Requisição Autenticada

```javascript
fetch('http://localhost:8080/api/profiles/me', {
  method: 'GET',
  headers: {
    'Authorization': 'Bearer seu-token-aqui'
  },
  credentials: 'include'  // Importante para cookies/tokens
})
  .then(res => res.json())
  .then(data => console.log(data));
```

### 3. Verificar Headers na Resposta

Abra DevTools → Network → Clique na requisição → Headers

Procure por:
- `Access-Control-Allow-Origin: http://localhost:3000`
- `Access-Control-Allow-Credentials: true`

---

## 📚 Referências

- [MDN - CORS](https://developer.mozilla.org/en-US/docs/Web/HTTP/CORS)
- [ASP.NET Core - CORS](https://learn.microsoft.com/en-us/aspnet/core/security/cors)
- [CORS Explained](https://www.codecademy.com/article/what-is-cors)

---

## 🎯 Resumo

1. **CORS é necessário** quando front-end e API estão em origens diferentes
2. **Configure no `Program.cs`** com `AddCors()` e `UseCors()`
3. **Ordem importa:** CORS antes de Authentication
4. **Use origens específicas** em produção, não `AllowAnyOrigin()`
5. **Teste no navegador**, não apenas no Postman

**🚀 Agora você está pronto para conectar seu front-end à API!**
