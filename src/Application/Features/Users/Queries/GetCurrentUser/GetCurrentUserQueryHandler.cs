using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Constants;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Users.Queries.GetCurrentUser;

/// <summary>
/// Đọc claim sub/email từ HttpContext, map sang Users và danh sách role.
/// </summary>
public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, Result<CurrentUserDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetCurrentUserQueryHandler(
        IApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<CurrentUserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var http = _httpContextAccessor.HttpContext;
        if (http?.User.Identity?.IsAuthenticated != true)
            return Result<CurrentUserDto>.Failure(ErrorCodes.UNAUTHORIZED);

        var sub = http.User.FindFirst("sub")?.Value
            ?? http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId))
            return Result<CurrentUserDto>.Failure(ErrorCodes.TOKEN_INVALID);

        var dbUser = await _context.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);
        if (dbUser == null)
            return Result<CurrentUserDto>.Failure(ErrorCodes.USER_NOT_FOUND);

        var roles = await _context.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == dbUser.UserId)
            .Join(_context.Roles, ur => ur.RoleId, r => r.RoleId, (ur, r) => new { r.Name, r.IsActive })
            .Where(x => x.IsActive)
            .Select(x => x.Name)
            .ToListAsync(cancellationToken);

        var username = http.User.FindFirst("unique_name")?.Value
            ?? dbUser.UserName;
        var email = http.User.FindFirst(ClaimTypes.Email)?.Value
            ?? http.User.FindFirst("email")?.Value
            ?? dbUser.Email;

        return Result<CurrentUserDto>.Success(new CurrentUserDto
        {
            Username = username,
            Email = email,
            Roles = roles.ToArray(),
        });
    }
}
