-- =====================================================================
-- HelpDesk - Dados iniciais (seed)
-- =====================================================================
-- Cria o usuário administrador padrão para o primeiro acesso.
-- Senha em texto puro: Admin@123  (troque assim que possível)
-- O hash abaixo foi gerado com BCrypt.Net-Next (mesmo algoritmo usado
-- pela API em Services/UserService.cs e Services/AuthService.cs).
--
-- Este mesmo usuário também é criado automaticamente pela API no
-- primeiro start (ver Data/DbSeeder.cs) caso ainda não exista nenhum
-- Admin no banco — rode este script apenas se estiver populando o
-- banco manualmente, sem passar pela aplicação.
-- =====================================================================

CREATE EXTENSION IF NOT EXISTS pgcrypto;

INSERT INTO users ("Id", "Name", "Email", "PasswordHash", "Role", "IsActive", "CreatedAt")
VALUES (
    gen_random_uuid(),
    'Administrador',
    'admin@helpdesk.local',
    '$2a$11$rso5lddTFctqyQisQf1B0Or/RX/AoDTAkx7Rg34QwfRtnjUJy0hSa',
    1, -- Admin
    true,
    now()
)
ON CONFLICT DO NOTHING;
