using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Constants;
using Shop_Cam_BE.Application.Common.Interfaces;

namespace Shop_Cam_BE.Application.Common.Helpers;

public static class UserRoleChecker
{
    public static async Task<bool> UserHasAdminRoleAsync(
        IApplicationDbContext db,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await db.UserRoles
            .AsNoTracking()
            .Join(db.Roles, ur => ur.RoleId, r => r.RoleId, (ur, r) => new { ur.UserId, r.NormalizedName, r.IsActive })
            .AnyAsync(
                x => x.UserId == userId && x.IsActive && x.NormalizedName == AppRoles.AdminNormalized,
                cancellationToken);
    }

    /// <summary>User có ít nhất một role trùng tên (không phân biệt hoa thường) với danh sách.</summary>
    public static async Task<bool> UserHasAnyRoleNameAsync(
        IApplicationDbContext db,
        Guid userId,
        IEnumerable<string> roleNames,
        CancellationToken cancellationToken = default)
    {
        var set = roleNames.Select(r => r.Trim()).Where(s => s.Length > 0)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (set.Count == 0)
            return false;

        return await db.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .Join(db.Roles, ur => ur.RoleId, r => r.RoleId, (ur, r) => new { r.Name, r.IsActive })
            .Where(x => x.IsActive)
            .AnyAsync(x => set.Contains(x.Name), cancellationToken);
    }
}
