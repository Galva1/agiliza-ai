-- =====================================================================
<<<<<<< HEAD
-- Agiliza HelpDesk - Script de criação do schema (PostgreSQL)
=======
-- Agiliza.ai - Script de criação do schema (PostgreSQL)
>>>>>>> 974e11b1c3aaa77e243ec890700622933fdbd10d
-- =====================================================================
-- Este script reflete o modelo mapeado pelo Entity Framework Core em
-- backend/src/HelpDesk.Api (ver Data/AppDbContext.cs e Data/Migrations).
-- Fornecido para documentação e para permitir criar o banco manualmente
-- (psql, pgAdmin, DBeaver etc.) sem depender do `dotnet ef`.
--
-- Caso prefira deixar a aplicação criar/atualizar o schema automaticamente,
-- basta subir o Postgres (docker compose up -d) e iniciar a API: o
-- Program.cs roda as migrations (Database.MigrateAsync) no startup.
--
-- Convenção de nomenclatura adotada:
--   - Tabelas no singular, em português.
--   - Chave primária = "id" + nome da tabela (idusuario, idticket, idticket_comentario).
--     Exceção: ticket_status usa apenas "idstatus".
--   - Colunas de auditoria presentes em (quase) toda tabela:
--       dt_criacao      -> data de criação do registro
--       usr_criacao     -> quem criou o registro (FK para usuario, quando aplicável)
--       dt_alteracao    -> data da última alteração
--       usr_alteracao   -> quem fez a última alteração (FK para usuario)
--       ativo           -> flag de registro ativo/inativo
--   - Em ticket_comentario, usr_criacao já representa o autor do comentário
--     (não existe uma coluna separada de "autor").
-- =====================================================================

<<<<<<< HEAD
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
=======
-- Enums antigos (Role, Priority) permanecem como INTEGER nas tabelas:
--   usuario.role        1 = Admin | 2 = Agente | 3 = Solicitante
--   ticket.priority     1 = Baixa | 2 = Media | 3 = Alta | 4 = Urgente
-- Status deixou de ser enum e agora é a tabela ticket_status (ver abaixo),
-- justamente para permitir renomear ou adicionar status sem alterar código.

CREATE TABLE IF NOT EXISTS usuario (
    idusuario       uuid PRIMARY KEY,
    nome            varchar(150) NOT NULL,
    email           varchar(200) NOT NULL,
    "senhaHash"     text NOT NULL,
    role            integer NOT NULL,
    ativo           boolean NOT NULL DEFAULT true,
    dt_criacao      timestamptz NOT NULL DEFAULT now(),
    usr_criacao     uuid NULL REFERENCES usuario ("idusuario") ON DELETE RESTRICT,
    dt_alteracao    timestamptz NULL,
    usr_alteracao   uuid NULL REFERENCES usuario ("idusuario") ON DELETE RESTRICT
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_usuario_email" ON usuario (email);

CREATE TABLE IF NOT EXISTS ticket_status (
    idstatus    integer GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
    nm_status   varchar(50) NOT NULL,
    dt_criacao  timestamptz NOT NULL DEFAULT now(),
    ativo       boolean NOT NULL DEFAULT true
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_ticket_status_nm_status" ON ticket_status (nm_status);

CREATE TABLE IF NOT EXISTS ticket (
    idticket                uuid PRIMARY KEY,
    titulo                   varchar(200) NOT NULL,
    descricao                text NOT NULL,
    idstatus                 integer NOT NULL REFERENCES ticket_status (idstatus) ON DELETE RESTRICT,
    priority                 integer NOT NULL DEFAULT 2,
    idusuario_solicitante    uuid NOT NULL REFERENCES usuario (idusuario) ON DELETE RESTRICT,
    idusuario_responsavel    uuid NULL REFERENCES usuario (idusuario) ON DELETE RESTRICT,
    ativo                    boolean NOT NULL DEFAULT true,
    dt_criacao               timestamptz NOT NULL DEFAULT now(),
    usr_criacao              uuid NULL REFERENCES usuario (idusuario) ON DELETE RESTRICT,
    dt_alteracao             timestamptz NULL,
    usr_alteracao            uuid NULL REFERENCES usuario (idusuario) ON DELETE RESTRICT,
    dt_fechamento            timestamptz NULL
);

CREATE INDEX IF NOT EXISTS "IX_ticket_idstatus" ON ticket (idstatus);
CREATE INDEX IF NOT EXISTS "IX_ticket_idusuario_solicitante" ON ticket (idusuario_solicitante);
CREATE INDEX IF NOT EXISTS "IX_ticket_idusuario_responsavel" ON ticket (idusuario_responsavel);

CREATE TABLE IF NOT EXISTS ticket_comentario (
    idticket_comentario  uuid PRIMARY KEY,
    idticket             uuid NOT NULL REFERENCES ticket (idticket) ON DELETE CASCADE,
    mensagem             text NOT NULL,
    ativo                boolean NOT NULL DEFAULT true,
    dt_criacao           timestamptz NOT NULL DEFAULT now(),
    usr_criacao          uuid NOT NULL REFERENCES usuario (idusuario) ON DELETE RESTRICT, -- autor do comentário
    dt_alteracao         timestamptz NULL,
    usr_alteracao        uuid NULL REFERENCES usuario (idusuario) ON DELETE RESTRICT
);

CREATE INDEX IF NOT EXISTS "IX_ticket_comentario_idticket" ON ticket_comentario (idticket);
>>>>>>> 974e11b1c3aaa77e243ec890700622933fdbd10d
