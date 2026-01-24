# 🎨 Prompt Completo para Geração do Front-End - GymDogs

## 📋 Contexto do Projeto

**GymDogs** é uma aplicação web de gerenciamento e acompanhamento de treinos de academia. É uma rede social onde usuários podem:
- Registrar seus próprios treinos, séries e exercícios de forma organizada
- Visualizar perfis públicos de outros usuários e seus treinos
- Organizar exercícios em pastas personalizadas (ex: "Costas", "Peito", "Pernas")
- Acompanhar histórico completo de séries com pesos e repetições

**IMPORTANTE:** Este prompt é para gerar APENAS a interface visual e componentes. As conexões com a API serão configuradas posteriormente. Foque em criar uma UI completa, responsiva e seguindo o design system especificado.

---

## 🎨 Design System e Paleta de Cores

### Paleta de Cores

```css
/* Cores Principais */
--primary: #D0FD3E;           /* Verde-Limão Elétrico - Botões, ações, destaques */
--background: #1C1C1E;        /* Fundo principal (ou #000000 para OLED) */
--surface: #2C2C2E;           /* Cards, modais, superfícies elevadas */
--text-primary: #FFFFFF;       /* Texto principal */
--text-secondary: #A1A1A1;     /* Texto secundário, labels */

/* Cores de Suporte (para futuras features) */
--protein: #4ECDC4;           /* Verde suave - Proteínas */
--carbs: #FFB347;             /* Amarelo/Laranja - Carboidratos */
--fats: #FF6B9D;              /* Rosa/Roxo - Gorduras */
```

### Tipografia

- **Fonte Principal:** Inter, Satoshi ou SF Pro Display
- **Títulos:** Bold/ExtraBold (pesados) para sensação de força
- **Números:** Tamanho grande e negrito (calorias, pesos, séries)
- **Hierarquia:** Títulos grandes, subtítulos médios, corpo pequeno

### Estilo Visual

- **Border Radius:** 24px a 32px para cards e botões (visual "Bento Grid")
- **Espaçamento:** Generoso, respiração entre elementos
- **Sombras:** Sutis, para dar profundidade aos cards
- **Glassmorphism:** Efeito de vidro fosco em barras de navegação (opcional)

---

## 🏗️ Estrutura de Rotas e Telas

### 1. Autenticação (Públicas)

#### `/login` - Tela de Login
- **Layout:** Tela cheia, centralizada verticalmente
- **Componentes:**
  - Logo/Brand no topo
  - Campo de email (input grande, arredondado)
  - Campo de senha (input grande, arredondado, com ícone de olho para mostrar/ocultar)
  - Botão "Entrar" (cor primária #D0FD3E, grande, arredondado)
  - Link "Não tem conta? Cadastre-se"
- **Estilo:** Fundo escuro, inputs com fundo #2C2C2E, texto branco

#### `/register` - Tela de Cadastro
- **Layout:** Similar ao login, com mais campos
- **Componentes:**
  - Campo de username
  - Campo de email
  - Campo de senha
  - Campo de confirmar senha
  - Botão "Criar Conta" (cor primária)
  - Link "Já tem conta? Faça login"
- **Validação Visual:** Mostrar erros abaixo dos campos (texto vermelho suave)

---

### 2. Dashboard / Home (`/dashboard` ou `/`)

**Layout:** Bento Grid (blocos de diferentes tamanhos)

**Componentes:**

1. **Header**
   - Logo/Brand à esquerda
   - Avatar do usuário à direita (com dropdown de menu)
   - Ícone de notificações (placeholder)

2. **Bento Grid Principal:**
   - **Card Grande (2x2):** Lista de pastas de treino do usuário
     - Título: "Meus Treinos"
     - Grid de cards pequenos, cada um representando uma pasta
     - Cada card mostra: Nome da pasta, quantidade de exercícios, última atualização
     - Botão "+" flutuante para criar nova pasta
   
   - **Card Médio (1x2):** Estatísticas rápidas
     - Total de séries registradas hoje
     - Total de exercícios únicos
     - Peso total levantado (se aplicável)
   
   - **Card Médio (1x2):** Últimas séries registradas
     - Lista das últimas 3-5 séries
     - Mostra: Exercício, peso, reps, data
   
   - **Card Pequeno (1x1):** Ação rápida
     - Botão "Adicionar Série" (cor primária)
     - Ícone grande, texto pequeno

3. **Seção de Perfis Públicos (Scroll horizontal)**
   - Título: "Descobrir Treinos"
   - Cards horizontais com perfis públicos
   - Cada card mostra: Avatar, nome, quantidade de pastas
   - Ao clicar, vai para `/profiles/{profileId}`

---

### 3. Perfis (`/profiles`)

#### `/profiles` - Lista de Perfis Públicos
- **Layout:** Grid de cards (2 colunas no mobile, 3-4 no desktop)
- **Componentes:**
  - Barra de busca no topo (busca por username ou display name)
  - Cards de perfil:
    - Avatar circular grande
    - Nome/Display Name
    - Bio (truncada)
    - Badge "Público" ou "Privado"
    - Botão "Ver Perfil"
  - Filtros (opcional): Ordenar por mais recentes

#### `/profiles/:profileId` - Visualizar Perfil
- **Layout:** Página de perfil completa
- **Componentes:**
  - **Header do Perfil:**
    - Avatar grande
    - Display Name
    - Bio completa
    - Badge de visibilidade
    - Botão "Editar" (se for o próprio perfil)
  
  - **Seção de Pastas de Treino:**
    - Grid de cards de pastas
    - Cada card mostra: Nome, descrição, quantidade de exercícios
    - Ao clicar, vai para `/profiles/:profileId/workout-folders/:folderId`
  
  - **Estatísticas (se aplicável):**
    - Total de pastas
    - Total de exercícios
    - Total de séries registradas

#### `/profiles/me` - Meu Perfil (Edição)
- **Layout:** Similar ao visualizar, mas com campos editáveis
- **Componentes:**
  - Formulário de edição:
    - Campo Display Name (editável)
    - Campo Bio (textarea, editável)
    - Toggle de visibilidade (Público/Privado)
    - Botão "Salvar" (cor primária)
    - Botão "Cancelar"

---

### 4. Pastas de Treino (`/workout-folders`)

#### `/workout-folders` - Lista de Pastas
- **Layout:** Grid de cards (similar ao dashboard)
- **Componentes:**
  - Header com título "Minhas Pastas de Treino"
  - Botão "+ Nova Pasta" (flutuante ou no header)
  - Cards de pasta:
    - Nome da pasta
    - Descrição (se houver)
    - Quantidade de exercícios
    - Data de criação/atualização
    - Menu de ações (3 pontos): Editar, Deletar, Reordenar

#### `/workout-folders/new` - Criar Nova Pasta
- **Layout:** Modal ou página completa
- **Componentes:**
  - Formulário:
    - Campo Nome (obrigatório)
    - Campo Descrição (opcional, textarea)
    - Campo Ordem (número, opcional)
    - Botão "Criar" (cor primária)
    - Botão "Cancelar"

#### `/workout-folders/:folderId` - Detalhes da Pasta
- **Layout:** Página completa com lista de exercícios
- **Componentes:**
  - **Header:**
    - Nome da pasta
    - Descrição
    - Botão "Editar" (se for dono)
    - Botão "Adicionar Exercício"
  
  - **Lista de Exercícios:**
    - Cards de exercícios na pasta
    - Cada card mostra:
      - Nome do exercício
      - Descrição (se houver)
      - Quantidade de séries registradas
      - Botão "Ver Séries" ou "Adicionar Série"
      - Menu de ações: Remover da pasta, Reordenar
  
  - **Ações:**
    - Botão "Adicionar Exercício" abre modal de busca
    - Modal mostra catálogo de exercícios com busca

#### `/workout-folders/:folderId/edit` - Editar Pasta
- **Layout:** Similar ao criar, mas pré-preenchido
- **Componentes:**
  - Formulário pré-preenchido
  - Botão "Salvar"
  - Botão "Cancelar"
  - Botão "Deletar" (vermelho, no rodapé)

---

### 5. Exercícios (`/exercises`)

#### `/exercises` - Catálogo de Exercícios
- **Layout:** Grid de cards com busca
- **Componentes:**
  - **Barra de Busca:**
    - Input de busca grande
    - Ícone de lupa
    - Busca em tempo real (placeholder para API)
  
  - **Grid de Exercícios:**
    - Cards de exercício:
      - Nome do exercício (grande, bold)
      - Descrição (truncada)
      - Badge "Admin" (se for admin criando)
      - Botão "Adicionar ao Treino" (se estiver em contexto de pasta)
  
  - **Ações Admin (se for admin):**
    - Botão "+ Criar Exercício" (flutuante)
    - Menu de ações em cada card: Editar, Deletar

#### `/exercises/new` - Criar Exercício (Admin)
- **Layout:** Modal ou página
- **Componentes:**
  - Formulário:
    - Campo Nome (obrigatório)
    - Campo Descrição (opcional, textarea)
    - Botão "Criar"
    - Botão "Cancelar"

#### `/exercises/:exerciseId` - Detalhes do Exercício
- **Layout:** Página de detalhes
- **Componentes:**
  - Nome do exercício (grande)
  - Descrição completa
  - Estatísticas (se aplicável):
    - Quantas vezes foi usado
    - Em quantas pastas está
  - Botão "Adicionar ao Meu Treino" (abre modal de seleção de pasta)

---

### 6. Séries (`/sets` ou dentro de exercícios)

#### Visualização de Séries (dentro de `/workout-folders/:folderId/exercises/:exerciseId`)
- **Layout:** Lista de séries com formulário de adicionar
- **Componentes:**
  - **Header:**
    - Nome do exercício
    - Botão "Adicionar Série"
  
  - **Formulário de Adicionar Série:**
    - Campo Repetições (número, grande)
    - Campo Peso (decimal, grande, em kg)
    - Campo Número da Série (opcional, auto-incrementado)
    - Botão "Registrar Série" (cor primária, grande)
  
  - **Lista de Séries:**
    - Cards de série (ordenados por data, mais recentes primeiro):
      - Número da série (badge)
      - Repetições (número grande, bold)
      - Peso (número grande, bold, em kg)
      - Data/hora de registro
      - Menu de ações: Editar, Deletar
  
  - **Estatísticas:**
    - Peso máximo levantado
    - Média de repetições
    - Total de séries registradas

#### Editar Série (Modal)
- **Layout:** Modal sobreposto
- **Componentes:**
  - Formulário pré-preenchido
  - Campo Repetições (editável)
  - Campo Peso (editável)
  - Botão "Salvar"
  - Botão "Cancelar"
  - Botão "Deletar" (vermelho)

---

### 7. Navegação

#### Barra de Navegação Inferior (Floating)
- **Posição:** Fixa na parte inferior
- **Estilo:** Glassmorphism (fundo semi-transparente) ou preto sólido
- **Ícones:** Outline icons, minimalistas
- **Itens:**
  - 🏠 Home/Dashboard
  - 📁 Pastas de Treino
  - 💪 Exercícios
  - 👤 Perfil
  - 🔍 Buscar (opcional)

#### Menu de Usuário (Dropdown no Header)
- **Itens:**
  - Meu Perfil
  - Configurações (placeholder)
  - Sair

---

## 🧩 Componentes Reutilizáveis a Criar

### 1. **Card Component**
- Props: `title`, `description`, `onClick`, `actions`
- Estilo: Fundo #2C2C2E, border-radius 24px, padding generoso
- Variantes: Small, Medium, Large

### 2. **Button Component**
- Variantes:
  - Primary: Cor #D0FD3E, texto preto
  - Secondary: Fundo #2C2C2E, texto branco
  - Danger: Vermelho suave
  - Ghost: Transparente, apenas borda
- Tamanhos: Small, Medium, Large
- Estados: Default, Hover, Active, Disabled

### 3. **Input Component**
- Estilo: Fundo #2C2C2E, borda sutil, texto branco
- Variantes: Text, Email, Password, Number, Textarea
- Estados: Default, Focus, Error, Disabled
- Incluir: Label, Placeholder, Error message

### 4. **Progress Ring / Circular Progress**
- Para mostrar progresso (futuro: calorias, macros)
- Cor primária #D0FD3E
- Número grande no centro

### 5. **Badge Component**
- Para tags, status, contadores
- Cores: Primary, Secondary, Success, Warning, Danger
- Tamanhos: Small, Medium

### 6. **Modal Component**
- Fundo escuro semi-transparente
- Card centralizado com border-radius 24px
- Botão de fechar (X)
- Animações suaves de entrada/saída

### 7. **Search Bar Component**
- Input grande com ícone de lupa
- Placeholder claro
- Botão de limpar (quando há texto)

### 8. **Avatar Component**
- Circular
- Tamanhos: Small, Medium, Large
- Fallback: Iniciais do nome

### 9. **Empty State Component**
- Para quando não há dados
- Ícone grande
- Mensagem
- Call-to-action (botão)

### 10. **Loading State Component**
- Skeleton loaders (shimmer effect)
- Spinner (para ações)
- Cor primária #D0FD3E

---

## 📱 Responsividade

- **Mobile First:** Design pensado primeiro para mobile
- **Breakpoints:**
  - Mobile: < 768px (1 coluna)
  - Tablet: 768px - 1024px (2 colunas)
  - Desktop: > 1024px (3-4 colunas)
- **Touch Targets:** Mínimo 44x44px para botões
- **Scroll:** Suave, com momentum no mobile

---

## 🎯 Funcionalidades Específicas por Tela

### Dashboard
- ✅ Listar pastas de treino do usuário logado
- ✅ Mostrar estatísticas rápidas
- ✅ Mostrar últimas séries registradas
- ✅ Link para criar nova pasta
- ✅ Scroll horizontal de perfis públicos

### Perfis
- ✅ Buscar perfis públicos por username ou display name
- ✅ Visualizar perfil público (se permitido)
- ✅ Editar próprio perfil (display name, bio, visibilidade)
- ✅ Listar pastas de treino do perfil visualizado

### Pastas de Treino
- ✅ Criar nova pasta (nome, descrição, ordem)
- ✅ Listar pastas do usuário
- ✅ Editar pasta (nome, descrição)
- ✅ Deletar pasta (com confirmação)
- ✅ Reordenar pastas (drag & drop ou input numérico)
- ✅ Adicionar exercício à pasta (busca no catálogo)
- ✅ Remover exercício da pasta
- ✅ Reordenar exercícios na pasta

### Exercícios
- ✅ Listar catálogo de exercícios
- ✅ Buscar exercícios por nome
- ✅ Visualizar detalhes do exercício
- ✅ Criar exercício (admin apenas)
- ✅ Editar exercício (admin apenas)
- ✅ Deletar exercício (admin apenas)

### Séries
- ✅ Adicionar série (reps, peso, número opcional)
- ✅ Listar séries de um exercício (ordenadas por data)
- ✅ Editar série (reps, peso)
- ✅ Deletar série (com confirmação)
- ✅ Mostrar estatísticas (peso máximo, média, total)

---

## 🔐 Autenticação e Autorização

### Fluxo de Autenticação
1. Usuário faz login → recebe `token` e `refreshToken`
2. Token armazenado (localStorage/sessionStorage)
3. Token enviado no header: `Authorization: Bearer {token}`
4. Se token expirar, usar `refreshToken` para renovar

### Estados de Autenticação
- **Não autenticado:** Mostrar apenas `/login` e `/register`
- **Autenticado:** Acesso completo às rotas protegidas
- **Token expirado:** Redirecionar para login ou renovar automaticamente

### Proteção de Rotas
- Rotas públicas: `/login`, `/register`
- Rotas protegidas: Todas as outras
- Rotas admin: `/exercises/new`, `/exercises/:id/edit`, `/exercises/:id/delete`

---

## 📊 Estrutura de Dados (DTOs da API)

### User/Profile
```typescript
interface User {
  id: string;
  username: string;
  email: string;
  role: "Admin" | "User";
}

interface Profile {
  id: string;
  userId: string;
  displayName: string;
  bio?: string;
  visibility: "Public" | "Private";
  createdAt: string;
  lastUpdatedAt: string;
}
```

### WorkoutFolder
```typescript
interface WorkoutFolder {
  id: string;
  profileId: string;
  name: string;
  description?: string;
  order: number;
  createdAt: string;
  lastUpdatedAt: string;
}
```

### Exercise
```typescript
interface Exercise {
  id: string;
  name: string;
  description?: string;
  createdAt: string;
  lastUpdatedAt: string;
}
```

### FolderExercise
```typescript
interface FolderExercise {
  id: string;
  workoutFolderId: string;
  exerciseId: string;
  exercise: Exercise; // Navegação
  order: number;
  createdAt: string;
}
```

### ExerciseSet
```typescript
interface ExerciseSet {
  id: string;
  folderExerciseId: string;
  setNumber: number;
  reps: number;
  weight: number; // em kg
  createdAt: string;
  lastUpdatedAt: string;
}
```

### Login Response
```typescript
interface LoginResponse {
  token: string;
  refreshToken: string;
  userId: string;
  username: string;
  email: string;
  expiresAt: string;
  refreshTokenExpiresAt: string;
  role: "Admin" | "User";
}
```

---

## 🎨 Detalhes de Design Específicos

### Cards de Pasta de Treino
- Fundo: #2C2C2E
- Border-radius: 24px
- Padding: 24px
- Hover: Leve elevação (sombra)
- Ícone de pasta ou músculo (opcional)

### Cards de Exercício
- Similar ao card de pasta
- Destaque no nome (bold, grande)
- Descrição em texto secundário (#A1A1A1)
- Badge com quantidade de séries (se houver)

### Cards de Série
- Layout horizontal ou vertical
- Números grandes e bold (reps, peso)
- Data pequena e discreta
- Ações (editar/deletar) em menu de 3 pontos

### Formulários
- Labels acima dos inputs
- Inputs com fundo #2C2C2E
- Placeholders em #A1A1A1
- Foco: Borda ou outline na cor primária #D0FD3E
- Erros: Texto vermelho suave abaixo do input

### Botões
- Primary: Fundo #D0FD3E, texto preto, bold
- Tamanho mínimo: 48px altura
- Border-radius: 24px
- Hover: Leve brilho ou elevação
- Active: Leve redução de escala

### Modais
- Fundo: rgba(0, 0, 0, 0.7) - overlay escuro
- Card: Fundo #2C2C2E, border-radius 32px
- Padding: 32px
- Largura máxima: 90% (mobile), 500px (desktop)
- Animação: Fade in + scale up

---

## 🚀 Stack Tecnológica Sugerida

- **Framework:** React (Next.js) ou React Native (se mobile)
- **Styling:** Tailwind CSS ou Styled Components
- **State Management:** Context API ou Zustand (simples)
- **Routing:** React Router (se React) ou Next.js Router
- **Form Handling:** React Hook Form + Zod (validação)
- **HTTP Client:** Axios ou Fetch API
- **Icons:** Lucide React ou React Icons
- **Animations:** Framer Motion (opcional)

---

## 📝 Notas Importantes para a IA

1. **NÃO implementar conexões com API ainda** - Apenas criar a UI completa
2. **Usar dados mockados** - Criar dados de exemplo para visualização
3. **Focar na experiência visual** - Design system é prioritário
4. **Componentes reutilizáveis** - Criar biblioteca de componentes
5. **Responsividade** - Mobile first, mas desktop também
6. **Acessibilidade básica** - Labels, alt texts, navegação por teclado
7. **Performance visual** - Animações suaves, loading states
8. **Consistência** - Mesmo estilo em todas as telas

---

## 🎯 Checklist de Telas a Criar

### Autenticação
- [ ] `/login` - Tela de login
- [ ] `/register` - Tela de cadastro

### Dashboard
- [ ] `/` ou `/dashboard` - Home com Bento Grid

### Perfis
- [ ] `/profiles` - Lista de perfis públicos
- [ ] `/profiles/search` - Busca de perfis
- [ ] `/profiles/:id` - Visualizar perfil
- [ ] `/profiles/me` - Meu perfil (edição)

### Pastas de Treino
- [ ] `/workout-folders` - Lista de pastas
- [ ] `/workout-folders/new` - Criar pasta
- [ ] `/workout-folders/:id` - Detalhes da pasta
- [ ] `/workout-folders/:id/edit` - Editar pasta

### Exercícios
- [ ] `/exercises` - Catálogo de exercícios
- [ ] `/exercises/search` - Busca de exercícios
- [ ] `/exercises/:id` - Detalhes do exercício
- [ ] `/exercises/new` - Criar exercício (admin)

### Séries
- [ ] `/workout-folders/:folderId/exercises/:exerciseId` - Visualizar e adicionar séries
- [ ] Modal de editar série
- [ ] Modal de adicionar série

### Componentes Globais
- [ ] Barra de navegação inferior
- [ ] Header com menu de usuário
- [ ] Modal genérico
- [ ] Loading states
- [ ] Empty states
- [ ] Error states

---

## 💡 Exemplo de Estrutura de Pastas (Sugestão)

```
src/
├── components/
│   ├── ui/
│   │   ├── Button.tsx
│   │   ├── Card.tsx
│   │   ├── Input.tsx
│   │   ├── Modal.tsx
│   │   ├── Badge.tsx
│   │   └── ...
│   ├── layout/
│   │   ├── Header.tsx
│   │   ├── BottomNav.tsx
│   │   └── Layout.tsx
│   └── features/
│       ├── auth/
│       ├── profiles/
│       ├── workout-folders/
│       ├── exercises/
│       └── exercise-sets/
├── pages/ ou app/ (dependendo do framework)
├── hooks/
├── utils/
├── types/
└── styles/
```

---

## 🔌 Endpoints da API (Para Referência Futura)

### Autenticação
- `POST /api/auth/register` - Registrar usuário
- `POST /api/auth/login` - Fazer login
- `POST /api/auth/refresh` - Renovar token

### Perfis
- `GET /api/profiles/{profileId}` - Obter perfil por ID
- `GET /api/profiles/user/{userId}` - Obter perfil por userId
- `GET /api/profiles/public` - Listar perfis públicos
- `GET /api/profiles/public/search?searchTerm={termo}` - Buscar perfis públicos
- `PUT /api/profiles/{profileId}` - Atualizar perfil
- `PUT /api/profiles/{profileId}/visibility` - Atualizar visibilidade

### Pastas de Treino
- `POST /api/profiles/{profileId}/workout-folders` - Criar pasta
- `GET /api/profiles/{profileId}/workout-folders` - Listar pastas
- `GET /api/profiles/{profileId}/workout-folders/{folderId}` - Obter pasta
- `PUT /api/profiles/{profileId}/workout-folders/{folderId}` - Atualizar pasta
- `PUT /api/profiles/{profileId}/workout-folders/{folderId}/order` - Atualizar ordem
- `DELETE /api/profiles/{profileId}/workout-folders/{folderId}` - Deletar pasta

### Exercícios (Catálogo)
- `GET /api/exercises` - Listar todos os exercícios
- `GET /api/exercises/search?searchTerm={termo}` - Buscar exercícios
- `GET /api/exercises/{id}` - Obter exercício
- `POST /api/exercises` - Criar exercício (Admin)
- `PUT /api/exercises/{id}` - Atualizar exercício (Admin)
- `DELETE /api/exercises/{id}` - Deletar exercício (Admin)

### Exercícios em Pastas
- `POST /api/workout-folders/{workoutFolderId}/exercises` - Adicionar exercício à pasta
- `GET /api/workout-folders/{workoutFolderId}/exercises` - Listar exercícios da pasta
- `GET /api/workout-folders/{workoutFolderId}/exercises/{folderExerciseId}` - Obter exercício da pasta
- `PUT /api/workout-folders/{workoutFolderId}/exercises/{folderExerciseId}/order` - Atualizar ordem
- `DELETE /api/workout-folders/{workoutFolderId}/exercises/{folderExerciseId}` - Remover exercício da pasta

### Séries
- `POST /api/folder-exercises/{folderExerciseId}/sets` - Adicionar série
- `GET /api/folder-exercises/{folderExerciseId}/sets` - Listar séries
- `GET /api/folder-exercises/{folderExerciseId}/sets/{setId}` - Obter série
- `PUT /api/folder-exercises/{folderExerciseId}/sets/{setId}` - Atualizar série
- `DELETE /api/folder-exercises/{folderExerciseId}/sets/{setId}` - Deletar série

---

## 🎨 Referência Visual

Use o design do Dribbble como referência principal, adaptando para o contexto de treinos de academia. Mantenha:
- ✅ Paleta de cores escura com acento vibrante
- ✅ Border radius generoso (24-32px)
- ✅ Tipografia bold para números e títulos
- ✅ Layout Bento Grid no dashboard
- ✅ Cards elevados com sombras sutis
- ✅ Navegação inferior flutuante

---

## ✅ Resultado Esperado

Ao final, você deve ter gerado:
1. ✅ Interface completa e funcional (com dados mockados)
2. ✅ Todos os componentes reutilizáveis
3. ✅ Todas as rotas/telas listadas
4. ✅ Design system aplicado consistentemente
5. ✅ Responsividade (mobile e desktop)
6. ✅ Animações e transições suaves
7. ✅ Estados de loading, empty e error
8. ✅ Formulários com validação visual
9. ✅ Navegação fluida entre telas
10. ✅ Código limpo e bem organizado

**Lembre-se:** Foque na experiência visual e na qualidade do código. As integrações com a API serão feitas posteriormente pelo desenvolvedor.

---

## 📚 Informações Adicionais

### Base URL da API
```
http://localhost:8080/api
```

### Autenticação
Todas as rotas protegidas requerem o header:
```
Authorization: Bearer {token}
```

### Formato de Resposta
A API retorna dados no formato JSON com estrutura padronizada usando `Ardalis.Result`:
- Sucesso: `{ "status": "Ok", "value": {...} }`
- Erro: `{ "status": "Error", "errors": [...] }`
- Validação: `{ "status": "Invalid", "errors": [...] }`

### Tratamento de Erros
- **400 Bad Request:** Dados inválidos
- **401 Unauthorized:** Token inválido ou expirado
- **403 Forbidden:** Sem permissão (ex: tentar editar perfil de outro usuário)
- **404 Not Found:** Recurso não encontrado
- **409 Conflict:** Conflito (ex: email já existe)
- **500 Internal Server Error:** Erro do servidor

---

## 🎯 Como Usar Este Prompt

1. **Copie todo o conteúdo deste README**
2. **Cole no Base44 ou Lovable** junto com o link do design de referência
3. **Adicione:** "Use este prompt completo para gerar o front-end da aplicação GymDogs"
4. **Especifique:** "Gere apenas a UI/design, não implemente conexões com API ainda"
5. **Aguarde a geração** e depois configure as conexões manualmente

---

**Boa sorte com a geração do front-end! 🚀**
