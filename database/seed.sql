-- =====================================================================
-- Agiliza.ai - Dados iniciais (seed)
-- =====================================================================
-- Popula os status padrão de chamado e cria o usuário administrador
-- inicial. Senha em texto puro do admin: Admin@123 (troque assim que
-- possível). O hash abaixo foi gerado com BCrypt.Net-Next (mesmo
-- algoritmo usado pela API em Services/UserService.cs e AuthService.cs).
--
-- Esses mesmos dados também são criados automaticamente pela API no
-- primeiro start (ver Data/DbSeeder.cs) caso ainda não existam — rode
-- este script apenas se estiver populando o banco manualmente, sem
-- passar pela aplicação.
-- =====================================================================

CREATE EXTENSION IF NOT EXISTS pgcrypto;

INSERT INTO ticket_status (nm_status, ativo)
VALUES
    ('Aberto', true),
    ('EmAndamento', true),
    ('Aguardando', true),
    ('Resolvido', true),
    ('Fechado', true)
ON CONFLICT (nm_status) DO NOTHING;

INSERT INTO usuario (idusuario, nome, email, "senhaHash", role, ativo, dt_criacao)
VALUES (
    gen_random_uuid(),
    'Administrador',
    'admin@helpdesk.local',
    '$2a$11$rso5lddTFctqyQisQf1B0Or/RX/AoDTAkx7Rg34QwfRtnjUJy0hSa',
    1, -- Admin
    true,
    now()
)
ON CONFLICT (email) DO NOTHING;
