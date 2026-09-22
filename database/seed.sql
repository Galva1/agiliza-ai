-- =====================================================================
<<<<<<< HEAD
-- Agiliza HelpDesk - Dados iniciais (seed)
-- =====================================================================
-- Cria o usuário administrador padrão para o primeiro acesso e as
-- categorias de chamado padrão.
-- Senha em texto puro do administrador: Admin@123 (troque assim que possível)
-- O hash abaixo foi gerado com BCrypt.Net-Next (mesmo algoritmo usado
-- pela API em Services/UsuarioService.cs e Services/AuthService.cs).
--
-- Este mesmo usuário e estas mesmas categorias também são criados
-- automaticamente pela API no primeiro start (ver Data/DbSeeder.cs)
-- caso ainda não existam — rode este script apenas se estiver populando
-- o banco manualmente, sem passar pela aplicação.
=======
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
>>>>>>> 974e11b1c3aaa77e243ec890700622933fdbd10d
-- =====================================================================

CREATE EXTENSION IF NOT EXISTS pgcrypto;

<<<<<<< HEAD
INSERT INTO usuarios (id_usuario, nm_usuario, ds_email, ds_senha_hash, id_perfil, fl_ativo, dt_cadastro)
=======
INSERT INTO ticket_status (nm_status, ativo)
VALUES
    ('Aberto', true),
    ('EmAndamento', true),
    ('Aguardando', true),
    ('Resolvido', true),
    ('Fechado', true)
ON CONFLICT (nm_status) DO NOTHING;

INSERT INTO usuario (idusuario, nome, email, "senhaHash", role, ativo, dt_criacao)
>>>>>>> 974e11b1c3aaa77e243ec890700622933fdbd10d
VALUES (
    gen_random_uuid(),
    'Administrador',
    'admin@helpdesk.local',
    '$2a$11$rso5lddTFctqyQisQf1B0Or/RX/AoDTAkx7Rg34QwfRtnjUJy0hSa',
    1, -- Administrador
    true,
    now()
)
<<<<<<< HEAD
ON CONFLICT DO NOTHING;

INSERT INTO categorias (id_categoria, nm_categoria, ds_categoria, fl_ativo, dt_cadastro)
VALUES
    (gen_random_uuid(), 'Hardware', 'Problemas com equipamentos físicos: computadores, impressoras, periféricos.', true, now()),
    (gen_random_uuid(), 'Software', 'Instalação, erros ou dúvidas sobre programas e sistemas.', true, now()),
    (gen_random_uuid(), 'Rede', 'Conectividade, Wi-Fi, VPN e acesso à internet.', true, now()),
    (gen_random_uuid(), 'Acesso e Permissões', 'Login, senhas e liberação de acesso a sistemas.', true, now()),
    (gen_random_uuid(), 'Outros', 'Demais solicitações que não se enquadram nas categorias acima.', true, now())
ON CONFLICT DO NOTHING;
=======
ON CONFLICT (email) DO NOTHING;
>>>>>>> 974e11b1c3aaa77e243ec890700622933fdbd10d
