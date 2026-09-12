-- =====================================================================
-- HelpDesk - Script de criação do schema (PostgreSQL)
-- =====================================================================
-- Este script reflete o modelo mapeado pelo Entity Framework Core em
-- backend/src/HelpDesk.Api (ver Data/AppDbContext.cs e Data/Migrations).
-- Ele é fornecido para documentação e para permitir criar o banco
-- manualmente (psql, pgAdmin, DBeaver etc.) sem depender do `dotnet ef`.
--
-- Caso prefira deixar a aplicação criar/atualizar o schema automaticamente,
-- basta subir o Postgres (docker compose up -d) e iniciar a API: o
-- Program.cs roda "dotnet ef database update" (MigrateAsync) no startup.
-- =====================================================================

-- Enums (armazenados como INTEGER nas tabelas, valores documentados aqui):
--
-- users."Role"            1 = Admin | 2 = Agente | 3 = Solicitante
-- tickets."Status"        1 = Aberto | 2 = EmAndamento | 3 = Aguardando | 4 = Resolvido | 5 = Fechado
-- tickets."Priority"      1 = Baixa | 2 = Media | 3 = Alta | 4 = Urgente

CREATE TABLE IF NOT EXISTS users (
    "Id"            uuid PRIMARY KEY,
    "Name"          varchar(150) NOT NULL,
    "Email"         varchar(200) NOT NULL,
    "PasswordHash"  text NOT NULL,
    "Role"          integer NOT NULL,
    "IsActive"      boolean NOT NULL DEFAULT true,
    "CreatedAt"     timestamptz NOT NULL DEFAULT now(),
    "UpdatedAt"     timestamptz NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_users_Email" ON users ("Email");

CREATE TABLE IF NOT EXISTS tickets (
    "Id"            uuid PRIMARY KEY,
    "Title"         varchar(200) NOT NULL,
    "Description"   text NOT NULL,
    "Status"        integer NOT NULL DEFAULT 1,
    "Priority"      integer NOT NULL DEFAULT 2,
    "RequesterId"   uuid NOT NULL REFERENCES users ("Id") ON DELETE RESTRICT,
    "AssigneeId"    uuid NULL REFERENCES users ("Id") ON DELETE RESTRICT,
    "CreatedAt"     timestamptz NOT NULL DEFAULT now(),
    "UpdatedAt"     timestamptz NULL,
    "ClosedAt"      timestamptz NULL
);

CREATE INDEX IF NOT EXISTS "IX_tickets_RequesterId" ON tickets ("RequesterId");
CREATE INDEX IF NOT EXISTS "IX_tickets_AssigneeId" ON tickets ("AssigneeId");

CREATE TABLE IF NOT EXISTS ticket_comments (
    "Id"            uuid PRIMARY KEY,
    "TicketId"      uuid NOT NULL REFERENCES tickets ("Id") ON DELETE CASCADE,
    "UserId"        uuid NOT NULL REFERENCES users ("Id") ON DELETE RESTRICT,
    "Message"       text NOT NULL,
    "CreatedAt"     timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX IF NOT EXISTS "IX_ticket_comments_TicketId" ON ticket_comments ("TicketId");
CREATE INDEX IF NOT EXISTS "IX_ticket_comments_UserId" ON ticket_comments ("UserId");
