using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Domain.Entities;
using System.Security.Claims;

namespace Shop_Cam_BE.Application.Common.Extensions;

public static class HttpContextExtensions
{
    public static Guid? GetCurrentUserId(this HttpContext httpContext, IApplicationDbContext context)
    {
        var sub = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? httpContext.User.FindFirst("sub")?.Value;
        if (sub == null || !Guid.TryParse(sub, out var userId))
            return null;

        return context.Users.AsNoTracking().Any(u => u.UserId == userId) ? userId : null;
    }

    public static User? GetCurrentUser(this HttpContext httpContext, IApplicationDbContext context)
    {
        var sub = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? httpContext.User.FindFirst("sub")?.Value;
        if (sub == null || !Guid.TryParse(sub, out var userId))
            return null;

        return context.Users.FirstOrDefault(x => x.UserId == userId);
    }
}
