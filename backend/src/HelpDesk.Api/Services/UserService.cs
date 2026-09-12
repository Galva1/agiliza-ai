using HelpDesk.Api.Common.Exceptions;
using HelpDesk.Api.Common.Mappings;
using HelpDesk.Api.Data;
using HelpDesk.Api.DTOs.Users;
using HelpDesk.Api.Entities;
using HelpDesk.Api.Entities.Enums;
using HelpDesk.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Api.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        return await _db.Users
            .OrderBy(u => u.Name)
            .Select(u => u.ToDto())
            .ToListAsync();
    }

    public async Task<UserDto> GetByIdAsync(Guid id)
    {
        var user = await FindUserOrThrowAsync(id);
        return user.ToDto();
    }

    public async Task<UserDto> CreateAsync(CreateUserRequest request)
    {
        var email = request.Email.Trim().ToLower();

        var emailInUse = await _db.Users.AnyAsync(u => u.Email == email);
        if (emailInUse)
        {
            throw new BusinessRuleException("Já existe um usuário com este e-mail.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return user.ToDto();
    }

    public async Task<UserDto> UpdateRoleAsync(Guid id, UserRole role)
    {
        var user = await FindUserOrThrowAsync(id);
        user.Role = role;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return user.ToDto();
    }

    public async Task<UserDto> UpdateStatusAsync(Guid id, bool isActive)
    {
        var user = await FindUserOrThrowAsync(id);
        user.IsActive = isActive;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return user.ToDto();
    }

    public async Task<UserDto> UpdateMyProfileAsync(Guid id, UpdateMyProfileRequest request)
    {
        var user = await FindUserOrThrowAsync(id);
        user.Name = request.Name.Trim();

        if (!string.IsNullOrWhiteSpace(request.NewPassword))
        {
            if (string.IsNullOrWhiteSpace(request.CurrentPassword) ||
                !BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            {
                throw new BusinessRuleException("Senha atual inválida.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        }

        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return user.ToDto();
    }

    public async Task<List<UserDto>> GetAssignableAgentsAsync()
    {
        return await _db.Users
            .Where(u => u.IsActive && (u.Role == UserRole.Agente || u.Role == UserRole.Admin))
            .OrderBy(u => u.Name)
            .Select(u => u.ToDto())
            .ToListAsync();
    }

    private async Task<User> FindUserOrThrowAsync(Guid id)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user is null)
        {
            throw new NotFoundException("Usuário não encontrado.");
        }
        return user;
    }
}
