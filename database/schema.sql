-- =====================================================================
-- Agiliza HelpDesk - Script de criação do schema (PostgreSQL)
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
-- usuarios.id_perfil         1 = Administrador | 2 = Tecnico | 3 = Solicitante
-- chamados.id_status         1 = Aberto | 2 = EmAndamento | 3 = Aguardando | 4 = Resolvido | 5 = Fechado
-- chamados.id_prioridade     1 = Baixa | 2 = Media | 3 = Alta | 4 = Urgente

CREATE TABLE IF NOT EXISTS usuarios (
    id_usuario      uuid PRIMARY KEY,
    nm_usuario      varchar(150) NOT NULL,
    ds_email        varchar(200) NOT NULL,
    ds_senha_hash   text NOT NULL,
    id_perfil       integer NOT NULL,
    fl_ativo        boolean NOT NULL DEFAULT true,
    dt_cadastro     timestamptz NOT NULL DEFAULT now(),
    dt_atualizacao  timestamptz NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_usuarios_ds_email" ON usuarios (ds_email);

CREATE TABLE IF NOT EXISTS categorias (
    id_categoria    uuid PRIMARY KEY,
    nm_categoria    varchar(100) NOT NULL,
    ds_categoria    text NULL,
    fl_ativo        boolean NOT NULL DEFAULT true,
    dt_cadastro     timestamptz NOT NULL DEFAULT now()
);

CREATE TABLE IF NOT EXISTS chamados (
    id_chamado              uuid PRIMARY KEY,
    nm_titulo               varchar(200) NOT NULL,
    ds_descricao            text NOT NULL,
    id_status               integer NOT NULL DEFAULT 1,
    id_prioridade           integer NOT NULL DEFAULT 2,
    id_categoria            uuid NULL REFERENCES categorias (id_categoria) ON DELETE RESTRICT,
    id_usuario_solicitante  uuid NOT NULL REFERENCES usuarios (id_usuario) ON DELETE RESTRICT,
    id_usuario_tecnico      uuid NULL REFERENCES usuarios (id_usuario) ON DELETE RESTRICT,
    ds_resolucao            text NULL,
    fl_resolucao_aprovada   boolean NOT NULL DEFAULT false,
    dt_resolucao            timestamptz NULL,
    dt_conclusao            timestamptz NULL,
    dt_cadastro             timestamptz NOT NULL DEFAULT now(),
    dt_atualizacao          timestamptz NULL,
    dt_fechamento           timestamptz NULL
);

CREATE INDEX IF NOT EXISTS "IX_chamados_id_categoria" ON chamados (id_categoria);
CREATE INDEX IF NOT EXISTS "IX_chamados_id_usuario_solicitante" ON chamados (id_usuario_solicitante);
CREATE INDEX IF NOT EXISTS "IX_chamados_id_usuario_tecnico" ON chamados (id_usuario_tecnico);

CREATE TABLE IF NOT EXISTS comentarios_chamado (
    id_comentario   uuid PRIMARY KEY,
    id_chamado      uuid NOT NULL REFERENCES chamados (id_chamado) ON DELETE CASCADE,
    id_usuario      uuid NOT NULL REFERENCES usuarios (id_usuario) ON DELETE RESTRICT,
    ds_mensagem     text NOT NULL,
    dt_cadastro     timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX IF NOT EXISTS "IX_comentarios_chamado_id_chamado" ON comentarios_chamado (id_chamado);
CREATE INDEX IF NOT EXISTS "IX_comentarios_chamado_id_usuario" ON comentarios_chamado (id_usuario);

CREATE TABLE IF NOT EXISTS historico_chamado (
    id_historico        uuid PRIMARY KEY,
    id_chamado          uuid NOT NULL REFERENCES chamados (id_chamado) ON DELETE CASCADE,
    id_usuario          uuid NOT NULL REFERENCES usuarios (id_usuario) ON DELETE RESTRICT,
    nm_acao             varchar(40) NOT NULL,
    ds_valor_anterior   text NULL,
    ds_valor_novo       text NULL,
    dt_cadastro         timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX IF NOT EXISTS "IX_historico_chamado_id_chamado" ON historico_chamado (id_chamado);
CREATE INDEX IF NOT EXISTS "IX_historico_chamado_id_usuario" ON historico_chamado (id_usuario);
