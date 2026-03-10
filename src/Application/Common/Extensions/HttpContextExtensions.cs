using Microsoft.AspNetCore.Http;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Domain.Entities;
using System.Security.Claims;

namespace Shop_Cam_BE.Application.Common.Extensions;

public static class HttpContextExtensions
{
    public static Guid? GetCurrentUserId(this HttpContext httpContext, IApplicationDbContext context)
    {
        var keycloakId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (keycloakId != null && Guid.TryParse(keycloakId.Value, out var keycId))
        {
            return context.Users.FirstOrDefault(x => x.KeycloakId == keycId)?.UserId;
        }
        return null;
    }

    public static User? GetCurrentUser(this HttpContext httpContext, IApplicationDbContext context)
    {
        var keycloakId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (keycloakId != null && Guid.TryParse(keycloakId.Value, out var keycId))
        {
            return context.Users.FirstOrDefault(x => x.KeycloakId == keycId);
        }
        return null;
    }
}
