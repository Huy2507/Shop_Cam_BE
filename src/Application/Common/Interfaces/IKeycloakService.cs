using MediatR;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Common.Interfaces;

public interface IKeycloakService
{
    Task<Result<TokenResultDto>> LoginAsync(string username, string password);
    Task<Result<TokenResultDto>> RefreshTokenAsync(string refreshToken);
    Task<bool> LogoutUserAsync(string refreshToken);
    Task<Result<KeycloakUserWithRoles>> GetUserIdAndRolesByEmailAsync(string email);
    Task<Result<Unit>> ResetPasswordAsync(Guid userId, string newPassword);
    Task<Result<UserInfoDto>> GetUserInfoByUsernameAsync(string username);
    Task<bool> CheckUserHasAnyRoleAsync(string accessToken, IEnumerable<string> requiredRoles);
}
