using HelpDesk.Api.Entities;
using HelpDesk.Api.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        await db.Database.MigrateAsync();
        await SeedAdminAsync(db);
        await SeedCategoriasAsync(db);
    }

    private static async Task SeedAdminAsync(AppDbContext db)
    {
        var hasAdmin = await db.Usuarios.AnyAsync(u => u.Perfil == PerfilUsuario.Administrador);
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
        await db.SaveChangesAsync();
    }
}
