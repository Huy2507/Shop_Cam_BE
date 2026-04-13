using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.DTOs;
using Shop_Cam_BE.Domain.Entities;
using Shop_Cam_BE.Infrastructure.Authentication;

namespace Shop_Cam_BE.Infrastructure.Services;

public class JwtTokenService : IJwtTokenService
{
    public const string TokenKindClaim = "token_kind";
    private const string TokenKindRefresh = "refresh";
    private const string TokenKindAccess = "access";

    private readonly IConfiguration _config;
    private readonly ILogger<JwtTokenService> _logger;
    private readonly JwtSecurityTokenHandler _handler = new();
    private readonly TokenValidationParameters _validationParameters;

    public JwtTokenService(IConfiguration config, ILogger<JwtTokenService> logger)
    {
        _config = config;
        _logger = logger;
        _validationParameters = JwtConfiguration.CreateTokenValidationParameters(config);
    }

    private SymmetricSecurityKey SigningKey =>
        (SymmetricSecurityKey)_validationParameters.IssuerSigningKey!;

    private string Issuer => _config["Jwt:Issuer"] ?? "ShopCam";
    private string Audience => _config["Jwt:Audience"] ?? "ShopCam";

    private int AccessTokenMinutes =>
        int.TryParse(_config["Jwt:AccessTokenMinutes"], out var m) ? m : 480;

    private int RefreshTokenDays =>
        int.TryParse(_config["Jwt:RefreshTokenDays"], out var d) ? d : 30;

    public TokenResultDto CreateTokenPair(User user, IReadOnlyList<string> roleNames)
    {
        var access = CreateJwt(user, roleNames, TimeSpan.FromMinutes(AccessTokenMinutes), TokenKindAccess);
        var refresh = CreateJwt(user, Array.Empty<string>(), TimeSpan.FromDays(RefreshTokenDays), TokenKindRefresh);

        return new TokenResultDto
        {
            AccessToken = access,
            RefreshToken = refresh,
            Email = user.Email,
        };
    }

    private string CreateJwt(User user, IReadOnlyList<string> roleNames, TimeSpan lifetime, string tokenKind)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new(TokenKindClaim, tokenKind),
        };
        foreach (var r in roleNames)
            claims.Add(new Claim(ClaimTypes.Role, r));

        var creds = new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(lifetime),
            signingCredentials: creds);
        return _handler.WriteToken(token);
    }

    public ClaimsPrincipal? ValidateAccessToken(string token)
    {
        try
        {
            var principal = _handler.ValidateToken(token, _validationParameters, out _);
            var kind = principal.FindFirst(TokenKindClaim)?.Value;
            if (kind == TokenKindRefresh)
                return null;
            if (kind != TokenKindAccess)
                return null;
            return principal;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Access token invalid");
            return null;
        }
    }

    public bool TryValidateRefreshToken(string refreshToken, out Guid userId)
    {
        userId = default;
        try
        {
            var principal = _handler.ValidateToken(refreshToken, _validationParameters, out _);
            if (principal.FindFirst(TokenKindClaim)?.Value != TokenKindRefresh)
                return false;
            var sub = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            return Guid.TryParse(sub, out userId) && userId != Guid.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Refresh token invalid");
            return false;
        }
    }
}
