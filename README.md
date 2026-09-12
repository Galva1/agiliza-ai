# Agiliza.ai (TCC)

Sistema de HelpDesk simples chamado **Agiliza.ai**: front-end em React (Vite + TypeScript) e back-end em C# (.NET, ASP.NET Core Web API), com PostgreSQL como banco de dados.

## Estrutura do repositório

```
backend/                 Solução .NET (API)
  NuGet.Config            Fonte NuGet fixada em nuget.org (evita fontes corporativas locais)
  HelpDesk.slnx
  src/HelpDesk.Api/
    Controllers/          Endpoints HTTP (Auth, Users, Tickets)
    Entities/              Entidades de domínio (Usuario, Chamado, ChamadoComentario, StatusChamado) + Enums (Perfil, Prioridade)
    DTOs/                  Objetos de transferência (request/response)
    Services/              Regras de negócio (Auth, Users, Tickets) + geração de JWT
    Data/                  DbContext, Migrations (EF Core) e Seeder
    Common/                Exceptions, Middlewares, Mappings, Options, Extensions

frontend/                 Aplicação React (Vite + TypeScript)
  src/
    api/                   Cliente axios + chamadas por recurso (auth, users, tickets)
    context/               AuthContext (usuário logado, token JWT)
    components/            Layout, Navbar, ProtectedRoute, Badges
    pages/                 LoginPage, TicketsOverviewPage, TicketDetailPage, UserSettingsPage, ProfilePage
    types/                 Tipos TypeScript espelhando os DTOs da API

database/
  schema.sql               DDL documentado (equivalente ao gerado pelas migrations do EF Core)
  seed.sql                 Usuário administrador inicial (uso manual, opcional)

docker-compose.yml         PostgreSQL + pgAdmin para desenvolvimento
```

## Como rodar

### 1. Banco de dados (PostgreSQL via Docker)

```bash
docker compose up -d
```

Isso sobe:
- **Postgres** em `localhost:5433` (banco `helpdesk`, usuário `helpdesk`, senha `helpdesk`) — porta 5433 em vez da padrão 5432 para não conflitar com uma instância local do Postgres, caso exista uma na máquina
- **pgAdmin** em `http://localhost:5050` (login `admin@helpdesk.dev` / `admin`)

Não é necessário rodar `database/schema.sql` manualmente — a API aplica as migrations do Entity Framework automaticamente ao iniciar (veja `Program.cs`). Os arquivos em `database/` existem para documentação/consulta e para permitir montar o banco sem a API, se preciso.

### 2. Back-end (.NET)

```bash
cd backend/src/HelpDesk.Api
dotnet run
```

A API sobe em `http://localhost:5109` (perfil `http` do `launchSettings.json`) e expõe o Swagger em `/swagger` (ambiente de desenvolvimento). No primeiro start, cria automaticamente um usuário administrador:

- **E-mail:** `admin@helpdesk.local`
- **Senha:** `Admin@123`

### 3. Front-end (React)

```bash
cd frontend
npm install
npm run dev
```

Acesse `http://localhost:5173`. O front já está configurado (`.env`) para falar com a API em `http://localhost:5109/api`.

## Padrões aplicados no back-end

- **Camadas simples**: `Controllers` → `Services` (interfaces + implementação) → `Data` (EF Core / `AppDbContext`), com `DTOs` isolando o que trafega pela API das `Entities` do banco.
- **Autenticação stateless com JWT**: login em `POST /api/auth/login` gera um token assinado (HMAC-SHA256); os demais endpoints exigem `Authorization: Bearer <token>`.
- **Autorização por perfil (role)**: enum `Perfil` (`Admin`, `Agente`, `Solicitante`) via `[Authorize(Roles = ...)]` nos controllers e checagens extras nos services (ex.: um Solicitante só vê os próprios chamados).
- **Senhas com hash (BCrypt)**: nunca armazenadas em texto puro.
- **Tratamento de erros centralizado**: `ExceptionMiddleware` converte exceções de domínio (`NotFoundException`, `ForbiddenException`, `BusinessRuleException`, `AuthenticationException`) em respostas HTTP com status apropriado.
- **Migrations code-first**: o schema do banco é versionado em `Data/Migrations` e aplicado automaticamente (`Database.MigrateAsync`) ao subir a API.

## Convenção de nomenclatura do banco e das entidades

As entidades C# (pasta `Entities/`) e o schema do banco (PostgreSQL) estão em português. O nome da tabela nem sempre é igual ao nome da classe (o mapeamento fica em `AppDbContext.OnModelCreating`, via Fluent API):

| Entidade C#         | Tabela              | Observação |
|----------------------|---------------------|------------|
| `Usuario`             | `usuario`            | |
| `Chamado`             | `ticket`             | tabela manteve o nome `ticket` |
| `ChamadoComentario`   | `ticket_comentario`  | |
| `StatusChamado`       | `ticket_status`      | domínio de status (ver abaixo) |
| enum `Perfil`         | coluna `role` em `usuario` | |
| enum `Prioridade`     | coluna `priority` em `ticket` | |

Convenções de nome de coluna no banco:

- **Chave primária**: `id` + nome da tabela — `idusuario`, `idticket`, `idticket_comentario` (exceção: `ticket_status` usa só `idstatus`).
- **Colunas de auditoria**, presentes em quase toda tabela:
  - `dt_criacao` / `usr_criacao` — data e usuário que criou o registro
  - `dt_alteracao` / `usr_alteracao` — data e usuário da última alteração
  - `ativo` — flag de registro ativo/inativo
- Em `ticket_comentario`, `usr_criacao` (propriedade `AutorId` na entidade) já representa o autor do comentário — não existe coluna separada para isso.
- **Status do chamado** deixou de ser um enum fixo no código e virou a entidade/tabela `StatusChamado` / `ticket_status` (`idstatus`, `nm_status`, `dt_criacao`, `ativo`) — permite renomear ou adicionar status sem precisar alterar/compilar o back-end. A API expõe `GET /api/tickets/status` para listar os status ativos.
- **Prioridade** (enum `Prioridade`) e **perfil de usuário** (enum `Perfil`) continuam sendo colunas simples (inteiro), sem tabela própria — não foram pedidos como domínio dinâmico.
- Os **DTOs** (`DTOs/`) e o contrato JSON da API permanecem em inglês/camelCase (`name`, `email`, `role`, `title`, `status`...) — são a camada de contrato consumida pelo front-end e não foram traduzidos, só as entidades internas.

Ver [database/schema.sql](database/schema.sql) para o DDL completo comentado.

## Perfis de usuário (MVP)

| Perfil       | Pode fazer |
|--------------|------------|
| Solicitante  | Abrir chamados, ver e comentar apenas os próprios chamados |
| Agente       | Ver todos os chamados, assumir/atualizar status, prioridade e responsável, comentar |
| Admin        | Tudo o que o Agente pode, além de gerenciar usuários (criar, ativar/desativar, atribuir perfil) |

## Próximos passos

Este é o esqueleto inicial (entidades, autenticação, CRUD básico de chamados e usuários, telas principais). Funcionalidades específicas de cada tela (filtros avançados, anexos, notificações, histórico de status, etc.) serão detalhadas e implementadas nas próximas iterações.
