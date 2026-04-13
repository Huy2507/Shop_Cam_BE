using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Shop_Cam_BE.Infrastructure.Authentication;

public static class JwtConfiguration
{
    public static TokenValidationParameters CreateTokenValidationParameters(IConfiguration config)
    {
        var key = config["Jwt:SigningKey"] ?? throw new InvalidOperationException("Jwt:SigningKey is required (min 32 chars).");
        if (key.Length < 32)
            throw new InvalidOperationException("Jwt:SigningKey must be at least 32 characters.");

        var issuer = config["Jwt:Issuer"] ?? "ShopCam";
        var audience = config["Jwt:Audience"] ?? "ShopCam";

        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            NameClaimType = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.UniqueName,
            RoleClaimType = System.Security.Claims.ClaimTypes.Role,
        };
    }
}
