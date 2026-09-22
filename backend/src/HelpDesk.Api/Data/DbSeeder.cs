using HelpDesk.Api.Entities;
using HelpDesk.Api.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        await db.Database.MigrateAsync();
        await SeedTicketStatusesAsync(db);
        await SeedAdminAsync(db);
    }

<<<<<<< HEAD
        await SeedAdminAsync(db);
        await SeedCategoriasAsync(db);
=======
    private static async Task SeedTicketStatusesAsync(AppDbContext db)
    {
        var existingNames = await db.StatusChamados.Select(s => s.Nome).ToListAsync();

        string[] defaultStatuses =
        [
            StatusChamadoNomes.Aberto,
            StatusChamadoNomes.EmAndamento,
            StatusChamadoNomes.Aguardando,
            StatusChamadoNomes.Resolvido,
            StatusChamadoNomes.Fechado
        ];

        foreach (var name in defaultStatuses)
        {
            if (!existingNames.Contains(name))
            {
                db.StatusChamados.Add(new StatusChamado
                {
                    Nome = name,
                    Ativo = true,
                    DataCriacao = DateTime.UtcNow
                });
            }
        }

        await db.SaveChangesAsync();
>>>>>>> 974e11b1c3aaa77e243ec890700622933fdbd10d
    }

    private static async Task SeedAdminAsync(AppDbContext db)
    {
<<<<<<< HEAD
        var hasAdmin = await db.Usuarios.AnyAsync(u => u.Perfil == PerfilUsuario.Administrador);
=======
        var hasAdmin = await db.Usuarios.AnyAsync(u => u.Perfil == Perfil.Admin);
>>>>>>> 974e11b1c3aaa77e243ec890700622933fdbd10d
        if (hasAdmin)
        {
            return;
        }

        var admin = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = "Administrador",
            Email = "admin@helpdesk.local",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
<<<<<<< HEAD
            Perfil = PerfilUsuario.Administrador,
            Ativo = true,
            CriadoEm = DateTime.UtcNow
        };

        db.Usuarios.Add(admin);
        await db.SaveChangesAsync();
    }

    private static async Task SeedCategoriasAsync(AppDbContext db)
    {
        var hasCategorias = await db.Categorias.AnyAsync();
        if (hasCategorias)
        {
            return;
        }

        var categorias = new[]
        {
            new Categoria
            {
                Id = Guid.NewGuid(),
                Nome = "Hardware",
                Descricao = "Problemas com equipamentos físicos: computadores, impressoras, periféricos.",
                Ativa = true,
                CriadoEm = DateTime.UtcNow
            },
            new Categoria
            {
                Id = Guid.NewGuid(),
                Nome = "Software",
                Descricao = "Instalação, erros ou dúvidas sobre programas e sistemas.",
                Ativa = true,
                CriadoEm = DateTime.UtcNow
            },
            new Categoria
            {
                Id = Guid.NewGuid(),
                Nome = "Rede",
                Descricao = "Conectividade, Wi-Fi, VPN e acesso à internet.",
                Ativa = true,
                CriadoEm = DateTime.UtcNow
            },
            new Categoria
            {
                Id = Guid.NewGuid(),
                Nome = "Acesso e Permissões",
                Descricao = "Login, senhas e liberação de acesso a sistemas.",
                Ativa = true,
                CriadoEm = DateTime.UtcNow
            },
            new Categoria
            {
                Id = Guid.NewGuid(),
                Nome = "Outros",
                Descricao = "Demais solicitações que não se enquadram nas categorias acima.",
                Ativa = true,
                CriadoEm = DateTime.UtcNow
            }
        };

        db.Categorias.AddRange(categorias);
=======
            Perfil = Perfil.Admin,
            Ativo = true,
            DataCriacao = DateTime.UtcNow,
            CriadoPorId = null
        };

        db.Usuarios.Add(admin);
>>>>>>> 974e11b1c3aaa77e243ec890700622933fdbd10d
        await db.SaveChangesAsync();
    }
}
