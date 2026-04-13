using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shop_Cam_BE.Application.Common.Constants;
using Shop_Cam_BE.Application.Common.Extensions;
using Shop_Cam_BE.Application.Common.Helpers;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;
using Shop_Cam_BE.Domain.Enums;

namespace Shop_Cam_BE.Application.Features.Auth.Commands.RefreshToken;

/// <summary>
/// Xác thực refresh token, kiểm tra user active và quyền theo accessFrom (admin hoặc RoleAccess).
/// </summary>
public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, Result<TokenResultDto>>
{
    private readonly IJwtTokenService _jwt;
    private readonly ILogger<RefreshTokenHandler> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _config;
    private readonly IApplicationDbContext _context;
    private readonly IActivityLogService _activityLogService;

    public RefreshTokenHandler(
        IJwtTokenService jwt,
        ILogger<RefreshTokenHandler> logger,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration config,
        IApplicationDbContext context,
        IActivityLogService activityLogService)
    {
        _jwt = jwt;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _config = config;
        _context = context;
        _activityLogService = activityLogService;
    }

    public async Task<Result<TokenResultDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var refreshToken = _httpContextAccessor.HttpContext?.Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogWarning("Không tìm thấy refresh token trong cookie");
                return Result<TokenResultDto>.Failure(ErrorCodes.INVALID_REFRESH_TOKEN);
            }

            if (!_jwt.TryValidateRefreshToken(refreshToken, out var userId))
            {
                _logger.LogWarning("Refresh token không hợp lệ hoặc hết hạn");
                return Result<TokenResultDto>.Failure(ErrorCodes.REFRESH_TOKEN_FAILED);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);
            if (user == null || !user.IsActive)
                return Result<TokenResultDto>.Failure(ErrorCodes.UNAUTHORIZED_ROLE);

            var accessFrom = request.AccessFrom?.ToLower().Trim() ?? "app";

            if (accessFrom == "admin")
            {
                if (!await UserRoleChecker.UserHasAdminRoleAsync(_context, user.UserId, cancellationToken))
                {
                    _logger.LogWarning("Refresh admin: user không có role Admin trong DB.");
                    return Result<TokenResultDto>.Failure(ErrorCodes.UNAUTHORIZED_ROLE);
                }
            }
            else
            {
                var allowedRoles = _config.GetSection($"RoleAccess:{accessFrom}").Get<string[]>() ?? Array.Empty<string>();
                if (allowedRoles.Length == 0)
                    return Result<TokenResultDto>.Failure(ErrorCodes.FORBIDDEN);

                if (!await UserRoleChecker.UserHasAnyRoleNameAsync(_context, user.UserId, allowedRoles, cancellationToken))
                {
                    _logger.LogWarning("Refresh: token không đủ quyền cho {AccessFrom}", accessFrom);
                    return Result<TokenResultDto>.Failure(ErrorCodes.UNAUTHORIZED_ROLE);
                }
            }

            var roleNames = await AuthUserQueries.GetRoleNamesAsync(_context, user.UserId, cancellationToken);
            var pair = _jwt.CreateTokenPair(user, roleNames);
            await _activityLogService.LogUserActionAsync(user.UserId, ActivityAction.TokenRefreshed);

            return Result<TokenResultDto>.Success(pair);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi không xác định khi làm mới token");
            return Result<TokenResultDto>.Failure(ErrorCodes.SERVER_ERROR);
        }
    }
}
