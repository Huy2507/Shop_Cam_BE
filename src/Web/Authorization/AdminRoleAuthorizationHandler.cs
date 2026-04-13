using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Helpers;
using Shop_Cam_BE.Application.Common.Interfaces;

namespace Shop_Cam_BE.Web.Authorization;

public sealed class AdminRoleAuthorizationHandler : AuthorizationHandler<AdminRoleRequirement>
{
    private readonly IApplicationDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AdminRoleAuthorizationHandler(
        IApplicationDbContext db,
        IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AdminRoleRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
            return;

        var sub = context.User.FindFirst("sub")?.Value
            ?? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId))
            return;

        var http = _httpContextAccessor.HttpContext;
        var ct = http?.RequestAborted ?? CancellationToken.None;

        var user = await _db.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == userId, ct);
        if (user == null)
            return;

        if (await UserRoleChecker.UserHasAdminRoleAsync(_db, user.UserId, ct))
            context.Succeed(requirement);
    }
}
