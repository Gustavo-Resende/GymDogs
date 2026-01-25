# 🔌 Guia Completo de Integração com a API GymDogs

Este documento contém todas as informações necessárias para integrar o front-end com a API backend do GymDogs.

---

## 📋 Índice

1. [Informações Básicas](#informações-básicas)
2. [Autenticação](#autenticação)
3. [Estrutura de Respostas](#estrutura-de-respostas)
4. [Tratamento de Erros](#tratamento-de-erros)
5. [Endpoints Disponíveis](#endpoints-disponíveis)
6. [Fluxos Completos](#fluxos-completos)
7. [Estrutura de Dados (DTOs)](#estrutura-de-dados-dtos)
8. [CORS](#cors)
9. [Refresh Token Automático](#refresh-token-automático)
10. [Exemplos de Código](#exemplos-de-código)

---

## 📍 Informações Básicas

### URL Base da API

```
Desenvolvimento: http://localhost:8080/api
Produção: [A definir]
```

### Content-Type

Todas as requisições devem usar:
```
Content-Type: application/json
```

### Autenticação

A maioria dos endpoints requer autenticação via JWT Bearer Token:

```
Authorization: Bearer {seu_token_aqui}
```

---

## 🔐 Autenticação

### 1. Registro de Usuário

**Endpoint:** `POST /api/auth/register`

**Request Body:**
```json
{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "SecurePassword123!"
}
```

**Validações:**
- `username`: Obrigatório, string
- `email`: Obrigatório, formato de email válido
- `password`: Obrigatório, mínimo 6 caracteres

**Response (201 Created):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "username": "johndoe",
  "email": "john@example.com",
  "createdAt": "2024-01-15T10:30:00Z"
}
```

**Erros Possíveis:**
- `400 Bad Request`: Dados inválidos
  ```json
  {
    "status": "Invalid",
    "errors": [
      {
        "identifier": "Email",
        "errorMessage": "Email is required."
      }
    ]
  }
  ```
- `409 Conflict`: Email ou username já existe
  ```json
  {
    "status": "Conflict",
    "errorMessage": "A user with the given email already exists."
  }
  ```

---

### 2. Login

**Endpoint:** `POST /api/auth/login`

**Request Body:**
```json
{
  "email": "john@example.com",
  "password": "SecurePassword123!"
}
```

**Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "abc123def456...",
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "username": "johndoe",
  "email": "john@example.com",
  "expiresAt": "2024-01-15T10:45:00Z",
  "refreshTokenExpiresAt": "2024-01-22T10:30:00Z",
  "role": "User"
}
```

**⚠️ IMPORTANTE:** Salve todos esses dados no `localStorage`:
- `token` → `localStorage.setItem('token', ...)`
- `refreshToken` → `localStorage.setItem('refreshToken', ...)`
- `userId` → `localStorage.setItem('userId', ...)`
- `username` → `localStorage.setItem('username', ...)`
- `email` → `localStorage.setItem('email', ...)`
- `role` → `localStorage.setItem('role', ...)`

**Erros Possíveis:**
- `400 Bad Request`: Dados inválidos
- `401 Unauthorized`: Email ou senha incorretos
  ```json
  {
    "status": "Unauthorized",
    "errorMessage": "Invalid email or password."
  }
  ```

---

### 3. Refresh Token

**Endpoint:** `POST /api/auth/refresh`

**Request Body:**
```json
{
  "refreshToken": "abc123def456..."
}
```

**Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "xyz789uvw012...",
  "expiresAt": "2024-01-15T11:00:00Z",
  "refreshTokenExpiresAt": "2024-01-22T10:30:00Z"
}
```

**⚠️ IMPORTANTE:** Atualize o `token` e `refreshToken` no `localStorage` após receber a resposta.

**Erros Possíveis:**
- `400 Bad Request`: Refresh token inválido ou ausente
- `401 Unauthorized`: Refresh token expirado ou inválido
  ```json
  {
    "status": "Unauthorized",
    "errorMessage": "Invalid or expired refresh token."
  }
  ```

---

## 📦 Estrutura de Respostas

### Resposta de Sucesso

**Status 200 OK / 201 Created:**
```json
{
  "id": "...",
  "name": "...",
  // ... outros campos do DTO
}
```

### Resposta de Erro

A API sempre retorna erros no seguinte formato:

```json
{
  "status": "Invalid|NotFound|Unauthorized|Forbidden|Conflict|Error",
  "errorMessage": "Mensagem de erro descritiva",
  "errors": [  // Apenas para erros de validação (400)
    {
      "identifier": "Campo",
      "errorMessage": "Mensagem específica do campo"
    }
  ]
}
```

**Status HTTP e Status do Result:**
- `400 Bad Request` → `status: "Invalid"` (validação)
- `404 Not Found` → `status: "NotFound"`
- `401 Unauthorized` → `status: "Unauthorized"`
- `403 Forbidden` → `status: "Forbidden"`
- `409 Conflict` → `status: "Conflict"`
- `500 Internal Server Error` → `status: "Error"`

---

## ⚠️ Tratamento de Erros

### Exemplo de Tratamento no Front-end

```typescript
try {
  const response = await fetch('http://localhost:8080/api/auth/login', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ email, password }),
  });

  const data = await response.json();

  if (!response.ok) {
    // Erro da API
    if (data.status === 'Invalid' && data.errors) {
      // Erros de validação (400)
      data.errors.forEach((error: any) => {
        console.error(`${error.identifier}: ${error.errorMessage}`);
      });
    } else {
      // Outros erros
      console.error(data.errorMessage || 'An error occurred');
    }
    return;
  }

  // Sucesso
  console.log('Login successful:', data);
} catch (error) {
  // Erro de rede ou outro erro
  console.error('Network error:', error);
}
```

---

## 🛣️ Endpoints Disponíveis

### Autenticação

| Método | Endpoint | Autenticação | Descrição |
|--------|----------|--------------|-----------|
| POST | `/api/auth/register` | ❌ | Registrar novo usuário |
| POST | `/api/auth/login` | ❌ | Fazer login |
| POST | `/api/auth/refresh` | ❌ | Renovar token |

---

### Perfis

| Método | Endpoint | Autenticação | Descrição |
|--------|----------|--------------|-----------|
| GET | `/api/profiles/{profileId}` | ✅ | Obter perfil por ID |
| GET | `/api/profiles/user/{userId}` | ✅ | Obter perfil por userId |
| GET | `/api/profiles/public` | ❌ | Listar perfis públicos (com cache de 30s) |
| GET | `/api/profiles/public/search?searchTerm={termo}` | ❌ | Buscar perfis públicos (com cache de 30s) |
| PUT | `/api/profiles/{profileId}` | ✅ | Atualizar perfil |
| PUT | `/api/profiles/{profileId}/visibility` | ✅ | Atualizar visibilidade |

**⚠️ IMPORTANTE:** Os endpoints `/api/profiles/public` e `/api/profiles/public/search` agora retornam um objeto `GetProfilesResponseDto` em vez de um array direto. Veja a seção de DTOs abaixo.

**Request Body - Atualizar Perfil:**
```json
{
  "displayName": "João Silva",
  "bio": "Apaixonado por musculação"
}
```

**Request Body - Atualizar Visibilidade:**
```json
{
  "visibility": 1  // 1 = Public, 2 = Private
}
```

---

### Pastas de Treino (Workout Folders)

| Método | Endpoint | Autenticação | Descrição |
|--------|----------|--------------|-----------|
| POST | `/api/profiles/{profileId}/workout-folders` | ✅ | Criar pasta |
| GET | `/api/profiles/{profileId}/workout-folders` | ✅ | Listar pastas |
| GET | `/api/profiles/{profileId}/workout-folders/{folderId}` | ✅ | Obter pasta |
| PUT | `/api/profiles/{profileId}/workout-folders/{folderId}` | ✅ | Atualizar pasta |
| PUT | `/api/profiles/{profileId}/workout-folders/{folderId}/order` | ✅ | Atualizar ordem |
| DELETE | `/api/profiles/{profileId}/workout-folders/{folderId}` | ✅ | Deletar pasta |

**Request Body - Criar Pasta:**
```json
{
  "name": "Costas",
  "description": "Treino de costas completo",
  "order": 1
}
```

**Request Body - Atualizar Ordem:**
```json
{
  "order": 2
}
```

---

### Exercícios (Catálogo)

| Método | Endpoint | Autenticação | Descrição |
|--------|----------|--------------|-----------|
| GET | `/api/exercises` | ❌ | Listar todos os exercícios |
| GET | `/api/exercises/search?searchTerm={termo}` | ❌ | Buscar exercícios |
| GET | `/api/exercises/{id}` | ❌ | Obter exercício |
| POST | `/api/exercises` | ✅ Admin | Criar exercício |
| PUT | `/api/exercises/{id}` | ✅ Admin | Atualizar exercício |
| DELETE | `/api/exercises/{id}` | ✅ Admin | Deletar exercício |

**Request Body - Criar Exercício (Admin):**
```json
{
  "name": "Supino Reto",
  "description": "Exercício para peitoral"
}
```

---

### Exercícios em Pastas (Folder Exercises)

| Método | Endpoint | Autenticação | Descrição |
|--------|----------|--------------|-----------|
| POST | `/api/workout-folders/{workoutFolderId}/exercises` | ✅ | Adicionar exercício à pasta |
| GET | `/api/workout-folders/{workoutFolderId}/exercises` | ✅ | Listar exercícios da pasta |
| GET | `/api/workout-folders/{workoutFolderId}/exercises/{folderExerciseId}` | ✅ | Obter exercício da pasta |
| PUT | `/api/workout-folders/{workoutFolderId}/exercises/{folderExerciseId}/order` | ✅ | Atualizar ordem |
| DELETE | `/api/workout-folders/{workoutFolderId}/exercises/{folderExerciseId}` | ✅ | Remover exercício da pasta |

**Request Body - Adicionar Exercício:**
```json
{
  "exerciseId": "550e8400-e29b-41d4-a716-446655440000",
  "order": 1
}
```

**Request Body - Atualizar Ordem:**
```json
{
  "order": 2
}
```

---

### Séries (Exercise Sets)

| Método | Endpoint | Autenticação | Descrição |
|--------|----------|--------------|-----------|
| POST | `/api/folder-exercises/{folderExerciseId}/sets` | ✅ | Adicionar série |
| GET | `/api/folder-exercises/{folderExerciseId}/sets` | ✅ | Listar séries |
| GET | `/api/folder-exercises/{folderExerciseId}/sets/{setId}` | ✅ | Obter série |
| PUT | `/api/folder-exercises/{folderExerciseId}/sets/{setId}` | ✅ | Atualizar série |
| DELETE | `/api/folder-exercises/{folderExerciseId}/sets/{setId}` | ✅ | Deletar série |

**Request Body - Adicionar Série:**
```json
{
  "setNumber": 1,
  "reps": 12,
  "weight": 80.5
}
```

**Request Body - Atualizar Série:**
```json
{
  "reps": 10,
  "weight": 85.0
}
```

---

## 🔄 Fluxos Completos

### Fluxo 1: Registro e Login

```typescript
// 1. Registrar usuário
const registerResponse = await fetch('http://localhost:8080/api/auth/register', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    username: 'johndoe',
    email: 'john@example.com',
    password: 'SecurePassword123!'
  })
});

const userData = await registerResponse.json();

// 2. Fazer login automaticamente
const loginResponse = await fetch('http://localhost:8080/api/auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    email: 'john@example.com',
    password: 'SecurePassword123!'
  })
});

const loginData = await loginResponse.json();

// 3. Salvar tokens e dados do usuário
localStorage.setItem('token', loginData.token);
localStorage.setItem('refreshToken', loginData.refreshToken);
localStorage.setItem('userId', loginData.userId);
localStorage.setItem('username', loginData.username);
localStorage.setItem('email', loginData.email);
localStorage.setItem('role', loginData.role);
```

---

### Fluxo 2: Criar Treino Completo

```typescript
// 1. Obter perfil do usuário
const profileResponse = await fetch(
  `http://localhost:8080/api/profiles/user/${userId}`,
  {
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    }
  }
);
const profile = await profileResponse.json();

// 2. Criar pasta de treino
const folderResponse = await fetch(
  `http://localhost:8080/api/profiles/${profile.id}/workout-folders`,
  {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      name: 'Costas',
      description: 'Treino de costas completo',
      order: 1
    })
  }
);
const folder = await folderResponse.json();

// 3. Buscar exercício no catálogo
const exercisesResponse = await fetch(
  'http://localhost:8080/api/exercises/search?searchTerm=remada',
  {
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    }
  }
);
const exercises = await exercisesResponse.json();
const exercise = exercises[0]; // Primeiro resultado

// 4. Adicionar exercício à pasta
const folderExerciseResponse = await fetch(
  `http://localhost:8080/api/workout-folders/${folder.id}/exercises`,
  {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      exerciseId: exercise.id,
      order: 1
    })
  }
);
const folderExercise = await folderExerciseResponse.json();

// 5. Registrar série
const setResponse = await fetch(
  `http://localhost:8080/api/folder-exercises/${folderExercise.id}/sets`,
  {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      setNumber: 1,
      reps: 12,
      weight: 80.5
    })
  }
);
const set = await setResponse.json();
```

---

### Fluxo 3: Visualizar Perfil Público

```typescript
// 1. Buscar perfis públicos (NOVO FORMATO)
const publicProfilesResponse = await fetch(
  'http://localhost:8080/api/profiles/public',
  {
    headers: {
      'Content-Type': 'application/json'
    }
  }
);
const profilesData = await publicProfilesResponse.json();

// Verificar se há perfis ou mostrar mensagem
if (profilesData.isEmpty) {
  // Exibir mensagem amigável do backend
  console.log(profilesData.message); // "Nenhum perfil público cadastrado ainda..."
} else {
  // Renderizar perfis
  console.log(`Encontrados ${profilesData.totalCount} perfis`);
  profilesData.profiles.forEach(profile => {
    console.log(profile.displayName);
  });
}

// 2. Buscar perfis por termo (NOVO FORMATO)
const searchResponse = await fetch(
  'http://localhost:8080/api/profiles/public/search?searchTerm=joao',
  {
    headers: {
      'Content-Type': 'application/json'
    }
  }
);
const searchData = await searchResponse.json();

if (searchData.isEmpty) {
  // Exibir mensagem de busca sem resultados
  console.log(searchData.message); // "Nenhum perfil público encontrado para o termo 'joao'..."
} else {
  // Renderizar resultados
  searchData.profiles.forEach(profile => {
    console.log(profile.displayName);
  });
}

// 3. Buscar perfil específico (se público)
const profileResponse = await fetch(
  `http://localhost:8080/api/profiles/${profileId}`,
  {
    headers: {
      'Authorization': `Bearer ${token}`, // Opcional, mas recomendado
      'Content-Type': 'application/json'
    }
  }
);
const profile = await profileResponse.json();

// 4. Listar pastas de treino do perfil
const foldersResponse = await fetch(
  `http://localhost:8080/api/profiles/${profile.id}/workout-folders`,
  {
    headers: {
      'Authorization': `Bearer ${token}`, // Necessário se perfil for privado
      'Content-Type': 'application/json'
    }
  }
);
const folders = await foldersResponse.json();
```

---

## 📊 Estrutura de Dados (DTOs)

### User DTOs

**CreateUserDto:**
```typescript
{
  id: string;           // GUID
  username: string;
  email: string;
  createdAt: string;    // ISO 8601 DateTime
}
```

**LoginDto:**
```typescript
{
  token: string;                    // JWT Access Token
  refreshToken: string;             // Refresh Token
  userId: string;                   // GUID
  username: string;
  email: string;
  expiresAt: string;                // ISO 8601 DateTime (UTC)
  refreshTokenExpiresAt: string;     // ISO 8601 DateTime (UTC)
  role: string;                     // "User" ou "Admin"
}
```

**RefreshTokenDto:**
```typescript
{
  token: string;                    // Novo JWT Access Token
  refreshToken: string;             // Novo Refresh Token
  expiresAt: string;               // ISO 8601 DateTime (UTC)
  refreshTokenExpiresAt: string;    // ISO 8601 DateTime (UTC)
}
```

---

### Profile DTOs

**GetProfileDto:**
```typescript
{
  id: string;                       // GUID
  userId: string;                   // GUID
  displayName: string;
  bio: string | null;
  visibility: 1 | 2;                // 1 = Public, 2 = Private
  createdAt: string;                // ISO 8601 DateTime
  lastUpdatedAt: string;            // ISO 8601 DateTime
}
```

**GetProfilesResponseDto (NOVO):**
```typescript
{
  profiles: GetProfileDto[];        // Lista de perfis encontrados
  isEmpty: boolean;                 // true se não há perfis
  message: string | null;           // Mensagem informativa quando vazio
  totalCount: number;               // Total de perfis encontrados
}
```

**Exemplo de resposta quando há perfis:**
```json
{
  "profiles": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "userId": "660e8400-e29b-41d4-a716-446655440001",
      "displayName": "João Silva",
      "bio": "Apaixonado por musculação",
      "visibility": 1,
      "createdAt": "2024-01-15T10:30:00Z",
      "lastUpdatedAt": "2024-01-15T10:30:00Z"
    }
  ],
  "isEmpty": false,
  "message": null,
  "totalCount": 1
}
```

**Exemplo de resposta quando NÃO há perfis:**
```json
{
  "profiles": [],
  "isEmpty": true,
  "message": "Nenhum perfil público cadastrado ainda. Seja o primeiro a se cadastrar!",
  "totalCount": 0
}
```

**Exemplo de resposta quando busca não retorna resultados:**
```json
{
  "profiles": [],
  "isEmpty": true,
  "message": "Nenhum perfil público encontrado para o termo 'joao'. Tente buscar com outro termo.",
  "totalCount": 0
}
```

---

### WorkoutFolder DTOs

**GetWorkoutFolderDto:**
```typescript
{
  id: string;                       // GUID
  profileId: string;                // GUID
  name: string;
  description: string | null;
  order: number;
  createdAt: string;                // ISO 8601 DateTime
  lastUpdatedAt: string;            // ISO 8601 DateTime
}
```

---

### Exercise DTOs

**GetExerciseDto:**
```typescript
{
  id: string;                       // GUID
  name: string;
  description: string | null;
  createdAt: string;                // ISO 8601 DateTime
  lastUpdatedAt: string;            // ISO 8601 DateTime
}
```

---

### FolderExercise DTOs

**GetFolderExerciseDto:**
```typescript
{
  id: string;                       // GUID
  workoutFolderId: string;         // GUID
  exerciseId: string;               // GUID
  exerciseName: string;
  exerciseDescription: string | null;
  order: number;
  createdAt: string;                // ISO 8601 DateTime
  lastUpdatedAt: string;            // ISO 8601 DateTime
}
```

---

### ExerciseSet DTOs

**GetExerciseSetDto:**
```typescript
{
  id: string;                       // GUID
  folderExerciseId: string;         // GUID
  setNumber: number;
  reps: number;
  weight: number;                  // decimal (ex: 80.5)
  createdAt: string;                // ISO 8601 DateTime
  lastUpdatedAt: string;            // ISO 8601 DateTime
}
```

---

## 🌐 CORS

### Configuração

A API está configurada para aceitar requisições das seguintes origens:

- `http://localhost:3000` (Next.js padrão)
- `http://localhost:5173` (Vite padrão)
- `http://localhost:5174` (Vite alternativo)
- `http://localhost:5175` (Vite alternativo)

### Headers Permitidos

- `Content-Type`
- `Authorization`
- `X-Requested-With`

### Métodos Permitidos

- `GET`
- `POST`
- `PUT`
- `DELETE`
- `PATCH`
- `OPTIONS` (preflight)

### Credenciais

A API aceita credenciais (`AllowCredentials: true`), então você pode usar cookies se necessário.

---

## ⚡ Otimizações de Performance

### Compressão HTTP

A API comprime automaticamente todas as respostas usando **Brotli** ou **Gzip**, reduzindo o tamanho das respostas em até 70-80%.

**Como funciona:**
- O navegador envia header `Accept-Encoding: br, gzip`
- A API escolhe o melhor algoritmo suportado
- Resposta é comprimida automaticamente
- Navegador descomprime automaticamente

**Benefícios:**
- ✅ Respostas 70-80% menores
- ✅ Carregamento mais rápido
- ✅ Menos dados trafegados
- ✅ Transparente para o front-end (automático)

### Response Caching

Alguns endpoints têm cache de 30 segundos para melhorar performance:

- `GET /api/profiles/public` - Cache de 30s
- `GET /api/profiles/public/search?searchTerm={termo}` - Cache de 30s

**Como funciona:**
- Primeira requisição: Executa query normalmente
- Requisições subsequentes (dentro de 30s): Retorna resposta do cache (instantâneo)
- Após 30s: Cache expira, próxima requisição executa query novamente

**Headers de cache:**
```
Cache-Control: public,max-age=30
```

**⚠️ Nota:** Dados podem estar desatualizados por até 30 segundos. Para dados em tempo real, use endpoints sem cache.

---

## 🔄 Refresh Token Automático

### Implementação Recomendada

Implemente um interceptor que renova o token automaticamente quando receber `401 Unauthorized`:

```typescript
// apiClient.ts
import axios from 'axios';

const apiClient = axios.create({
  baseURL: 'http://localhost:8080/api',
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Interceptor de requisição - adiciona token
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Interceptor de resposta - renova token quando expira
apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    // Se receber 401 e ainda não tentou renovar
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        const refreshToken = localStorage.getItem('refreshToken');
        if (!refreshToken) {
          throw new Error('No refresh token available');
        }

        // Tentar renovar token
        const response = await axios.post(
          'http://localhost:8080/api/auth/refresh',
          { refreshToken }
        );

        const { token: newToken, refreshToken: newRefreshToken } = response.data;

        // Atualizar tokens no localStorage
        localStorage.setItem('token', newToken);
        localStorage.setItem('refreshToken', newRefreshToken);

        // Atualizar header e tentar novamente
        originalRequest.headers.Authorization = `Bearer ${newToken}`;
        return apiClient(originalRequest);
      } catch (refreshError) {
        // Refresh falhou - deslogar usuário
        localStorage.clear();
        window.location.href = '/login';
        return Promise.reject(refreshError);
      }
    }

    return Promise.reject(error);
  }
);

export default apiClient;
```

---

## 💻 Exemplos de Código

### Exemplo 1: Cliente HTTP Completo

```typescript
// lib/services/apiClient.ts
const API_BASE_URL = 'http://localhost:8080/api';

class ApiClient {
  private getAuthHeaders(): HeadersInit {
    const token = localStorage.getItem('token');
    return {
      'Content-Type': 'application/json',
      ...(token && { Authorization: `Bearer ${token}` }),
    };
  }

  async request<T>(
    endpoint: string,
    options: RequestInit = {}
  ): Promise<T> {
    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
      ...options,
      headers: {
        ...this.getAuthHeaders(),
        ...options.headers,
      },
    });

    const data = await response.json();

    if (!response.ok) {
      throw new ApiError(data.status, data.errorMessage, data.errors);
    }

    return data;
  }

  async get<T>(endpoint: string): Promise<T> {
    return this.request<T>(endpoint, { method: 'GET' });
  }

  async post<T>(endpoint: string, body: any): Promise<T> {
    return this.request<T>(endpoint, {
      method: 'POST',
      body: JSON.stringify(body),
    });
  }

  async put<T>(endpoint: string, body: any): Promise<T> {
    return this.request<T>(endpoint, {
      method: 'PUT',
      body: JSON.stringify(body),
    });
  }

  async delete<T>(endpoint: string): Promise<T> {
    return this.request<T>(endpoint, { method: 'DELETE' });
  }
}

class ApiError extends Error {
  constructor(
    public status: string,
    public message: string,
    public errors?: Array<{ identifier: string; errorMessage: string }>
  ) {
    super(message);
  }
}

export const apiClient = new ApiClient();
export { ApiError };
```

---

### Exemplo 2: Serviço de Autenticação

```typescript
// lib/services/authService.ts
import { apiClient } from './apiClient';

interface LoginRequest {
  email: string;
  password: string;
}

interface RegisterRequest {
  username: string;
  email: string;
  password: string;
}

interface LoginResponse {
  token: string;
  refreshToken: string;
  userId: string;
  username: string;
  email: string;
  expiresAt: string;
  refreshTokenExpiresAt: string;
  role: string;
}

export const authService = {
  async register(data: RegisterRequest) {
    return apiClient.post('/auth/register', data);
  },

  async login(data: LoginRequest): Promise<LoginResponse> {
    const response = await apiClient.post<LoginResponse>('/auth/login', data);
    
    // Salvar no localStorage
    localStorage.setItem('token', response.token);
    localStorage.setItem('refreshToken', response.refreshToken);
    localStorage.setItem('userId', response.userId);
    localStorage.setItem('username', response.username);
    localStorage.setItem('email', response.email);
    localStorage.setItem('role', response.role);
    
    return response;
  },

  async refreshToken(refreshToken: string) {
    const response = await apiClient.post('/auth/refresh', { refreshToken });
    
    // Atualizar tokens
    localStorage.setItem('token', response.token);
    localStorage.setItem('refreshToken', response.refreshToken);
    
    return response;
  },

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('userId');
    localStorage.removeItem('username');
    localStorage.removeItem('email');
    localStorage.removeItem('role');
  },

  isAuthenticated(): boolean {
    return !!localStorage.getItem('token');
  },
};
```

---

### Exemplo 3: Hook de Autenticação

```typescript
// hooks/useAuth.ts
import { useState, useEffect } from 'react';
import { authService } from '../lib/services/authService';

interface User {
  userId: string;
  username: string;
  email: string;
  role: string;
}

export const useAuth = () => {
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState<boolean>(true);

  useEffect(() => {
    checkAuth();
  }, []);

  const checkAuth = () => {
    const token = localStorage.getItem('token');
    const userId = localStorage.getItem('userId');
    const username = localStorage.getItem('username');
    const email = localStorage.getItem('email');
    const role = localStorage.getItem('role');

    if (token && userId) {
      setIsAuthenticated(true);
      setUser({
        userId,
        username: username || '',
        email: email || '',
        role: role || 'User',
      });
    } else {
      setIsAuthenticated(false);
      setUser(null);
    }
    setIsLoading(false);
  };

  const login = async (email: string, password: string) => {
    const response = await authService.login({ email, password });
    setIsAuthenticated(true);
    setUser({
      userId: response.userId,
      username: response.username,
      email: response.email,
      role: response.role,
    });
    return response;
  };

  const logout = () => {
    authService.logout();
    setIsAuthenticated(false);
    setUser(null);
  };

  return {
    isAuthenticated,
    user,
    isLoading,
    login,
    logout,
    checkAuth,
  };
};
```

---

### Exemplo 4: Buscar Perfis Públicos (NOVO FORMATO)

```typescript
// lib/services/profileService.ts
import { apiClient } from './apiClient';

interface GetProfileDto {
  id: string;
  userId: string;
  displayName: string;
  bio: string | null;
  visibility: 1 | 2;
  createdAt: string;
  lastUpdatedAt: string;
}

interface GetProfilesResponseDto {
  profiles: GetProfileDto[];
  isEmpty: boolean;
  message: string | null;
  totalCount: number;
}

export const profileService = {
  async getPublicProfiles(): Promise<GetProfilesResponseDto> {
    return apiClient.get<GetProfilesResponseDto>('/profiles/public');
  },

  async searchPublicProfiles(searchTerm: string): Promise<GetProfilesResponseDto> {
    return apiClient.get<GetProfilesResponseDto>(
      `/profiles/public/search?searchTerm=${encodeURIComponent(searchTerm)}`
    );
  },
};

// Uso no componente
async function loadPublicProfiles() {
  try {
    const data = await profileService.getPublicProfiles();
    
    if (data.isEmpty) {
      // Exibir mensagem amigável
      showMessage(data.message); // "Nenhum perfil público cadastrado ainda..."
    } else {
      // Renderizar perfis
      renderProfiles(data.profiles);
      showCount(data.totalCount);
    }
  } catch (error) {
    console.error('Erro ao carregar perfis:', error);
  }
}

async function searchProfiles(term: string) {
  try {
    const data = await profileService.searchPublicProfiles(term);
    
    if (data.isEmpty) {
      // Exibir mensagem de busca sem resultados
      showMessage(data.message); // "Nenhum perfil público encontrado para o termo 'term'..."
    } else {
      // Renderizar resultados
      renderProfiles(data.profiles);
      showCount(data.totalCount);
    }
  } catch (error) {
    console.error('Erro ao buscar perfis:', error);
  }
}
```

---

## ✅ Checklist de Implementação

- [ ] Configurar URL base da API
- [ ] Implementar cliente HTTP com interceptors
- [ ] Implementar refresh token automático
- [ ] Criar serviços para cada entidade (auth, profiles, workouts, etc.)
- [ ] Implementar hook `useAuth`
- [ ] Implementar proteção de rotas
- [ ] Tratar erros da API (400, 401, 403, 404, 409, 500)
- [ ] Exibir mensagens de erro amigáveis ao usuário
- [ ] Implementar estados de loading
- [ ] Salvar tokens no localStorage após login
- [ ] Limpar localStorage após logout
- [ ] Validar formulários antes de enviar
- [ ] **Atualizar endpoints de perfis públicos para usar `GetProfilesResponseDto`**
- [ ] **Tratar listas vazias usando `isEmpty` e `message` do backend**
- [ ] **Aproveitar compressão HTTP automática (já configurada)**
- [ ] **Considerar cache de 30s nos endpoints de listagem pública**

---

## 🚨 Problemas Comuns e Soluções

### Erro 401 Unauthorized

**Causa:** Token expirado ou ausente.

**Solução:**
1. Verificar se o token está sendo enviado no header `Authorization`
2. Implementar refresh token automático
3. Redirecionar para login se refresh falhar

---

### Erro 409 Conflict no Registro

**Causa:** Email ou username já existe.

**Solução:**
- Exibir mensagem: "Este email já está cadastrado. Tente fazer login ou use outro email."
- Verificar se o usuário já tem conta antes de mostrar formulário de registro

---

### Erro CORS

**Causa:** Origem não permitida ou configuração incorreta.

**Solução:**
- Verificar se a URL do front-end está na lista de origens permitidas
- Verificar se está usando `http://` e não `https://` em desenvolvimento
- Verificar se o Docker foi reconstruído após mudanças de CORS

---

### Token não está sendo enviado

**Causa:** Token não está no localStorage ou header não está sendo adicionado.

**Solução:**
- Verificar se `localStorage.getItem('token')` retorna um valor
- Verificar se o interceptor está adicionando o header corretamente
- Verificar se o token não expirou

---

### Lista de perfis vazia retorna erro

**Causa:** Front-end esperando array direto, mas API retorna objeto `GetProfilesResponseDto`.

**Solução:**
- Atualizar código para usar `GetProfilesResponseDto`:
  ```typescript
  // ❌ ANTES (errado)
  const profiles = await response.json(); // Espera array
  if (profiles.length === 0) { ... }
  
  // ✅ DEPOIS (correto)
  const data: GetProfilesResponseDto = await response.json();
  if (data.isEmpty) {
    showMessage(data.message); // Usa mensagem do backend
  } else {
    renderProfiles(data.profiles);
  }
  ```

---

### Respostas muito grandes (performance)

**Causa:** Respostas JSON não comprimidas.

**Solução:**
- A API já comprime automaticamente (Brotli/Gzip)
- Verificar header `Content-Encoding: br` ou `Content-Encoding: gzip` na resposta
- Se não estiver comprimindo, verificar se o navegador envia `Accept-Encoding: br, gzip`

---

## 📞 Suporte

Para dúvidas ou problemas, consulte:
- Documentação da API: `http://localhost:8080/scalar` (quando a API estiver rodando)
- README principal do projeto
- Logs do backend para debug

---

---

## 📝 Notas Importantes

### Mudanças Recentes (Janeiro 2024)

1. **Endpoints de Perfis Públicos Atualizados:**
   - `GET /api/profiles/public` agora retorna `GetProfilesResponseDto` (não mais array direto)
   - `GET /api/profiles/public/search` agora retorna `GetProfilesResponseDto` (não mais array direto)
   - Sempre verifique `isEmpty` antes de renderizar perfis
   - Use `message` para exibir feedback quando não há resultados

2. **Otimizações de Performance:**
   - Compressão HTTP automática (Brotli/Gzip)
   - Cache de 30s em endpoints de listagem pública
   - Queries otimizadas com `AsNoTracking()`

3. **Mensagens Informativas:**
   - Backend agora fornece mensagens prontas quando listas estão vazias
   - Não é mais necessário criar mensagens customizadas no front-end

---

**Última atualização:** Janeiro 2024  
**Versão da API:** 1.1.0  
**Changelog:** Ver `README.PERFORMANCE_IMPROVEMENTS.md` para detalhes completos das melhorias
