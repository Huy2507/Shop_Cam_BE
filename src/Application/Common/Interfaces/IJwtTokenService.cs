using System.Security.Claims;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;
using Shop_Cam_BE.Domain.Entities;

namespace Shop_Cam_BE.Application.Common.Interfaces;

/// <summary>Phát hành và kiểm tra JWT cục bộ (không Keycloak).</summary>
public interface IJwtTokenService
{
    TokenResultDto CreateTokenPair(User user, IReadOnlyList<string> roleNames);

    /// <summary>Kiểm tra access token; trả principal nếu hợp lệ.</summary>
    ClaimsPrincipal? ValidateAccessToken(string token);

    /// <summary>Kiểm tra refresh token; trả UserId nếu hợp lệ.</summary>
    bool TryValidateRefreshToken(string refreshToken, out Guid userId);
}
