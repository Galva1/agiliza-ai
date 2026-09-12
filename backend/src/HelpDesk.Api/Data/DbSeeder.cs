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
    }

    private static async Task SeedAdminAsync(AppDbContext db)
    {
        var hasAdmin = await db.Usuarios.AnyAsync(u => u.Perfil == Perfil.Admin);
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
            Perfil = Perfil.Admin,
            Ativo = true,
            DataCriacao = DateTime.UtcNow,
            CriadoPorId = null
        };

        db.Usuarios.Add(admin);
        await db.SaveChangesAsync();
    }
}
