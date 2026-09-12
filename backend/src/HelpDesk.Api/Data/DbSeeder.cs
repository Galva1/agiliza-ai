using HelpDesk.Api.Entities;
using HelpDesk.Api.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        await db.Database.MigrateAsync();

        var hasAdmin = await db.Users.AnyAsync(u => u.Role == UserRole.Admin);
        if (hasAdmin)
        {
            return;
        }

        var admin = new User
        {
            Id = Guid.NewGuid(),
            Name = "Administrador",
            Email = "admin@helpdesk.local",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = UserRole.Admin,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        db.Users.Add(admin);
        await db.SaveChangesAsync();
    }
}
