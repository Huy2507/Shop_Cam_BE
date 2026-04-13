using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Domain.Entities;

namespace Shop_Cam_BE.Application.Common.Helpers;

public static class AuthUserQueries
{
    public static async Task<User?> FindByUsernameOrEmailAsync(
        IApplicationDbContext db,
        string usernameOrEmail,
        CancellationToken cancellationToken = default)
    {
        var x = usernameOrEmail.Trim();
        return await db.Users
            .FirstOrDefaultAsync(u => u.UserName == x || u.Email == x, cancellationToken);
    }

    public static async Task<List<string>> GetRoleNamesAsync(
        IApplicationDbContext db,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await db.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .Join(db.Roles, ur => ur.RoleId, r => r.RoleId, (ur, r) => new { r.Name, r.IsActive })
            .Where(x => x.IsActive)
            .Select(x => x.Name)
            .ToListAsync(cancellationToken);
    }
}
