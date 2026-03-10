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

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, Result<TokenResultDto>>
{
    private readonly IKeycloakService _keycloakService;
    private readonly ILogger<RefreshTokenHandler> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _config;
    private readonly IApplicationDbContext _context;
    private readonly IActivityLogService _activityLogService;

    public RefreshTokenHandler(
        IKeycloakService keycloakService,
        ILogger<RefreshTokenHandler> logger,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration config,
        IApplicationDbContext context,
        IActivityLogService activityLogService)
    {
        _keycloakService = keycloakService;
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
                return Result<TokenResultDto>.Failure(ErrorCodes.INVALID_REFRESH_TOKEN, ["Không tìm thấy refresh token. Vui lòng đăng nhập lại."]);
            }

            var result = await _keycloakService.RefreshTokenAsync(refreshToken);
            if (!result.Succeeded || result.Value == null)
            {
                _logger.LogWarning("Làm mới token thất bại: {Message}", string.Join(", ", result.Errors));
                return Result<TokenResultDto>.Failure(ErrorCodes.REFRESH_TOKEN_FAILED, result.Errors);
            }

            var tokenDto = result.Value;
            var accessFrom = request.AccessFrom?.ToLower().Trim() ?? "app";
            var allowedRoles = _config.GetSection($"RoleAccess:{accessFrom}").Get<string[]>() ?? Array.Empty<string>();
            var hasRole = await _keycloakService.CheckUserHasAnyRoleAsync(tokenDto.AccessToken, allowedRoles);

            if (!hasRole)
            {
                _logger.LogWarning("Token mới không hợp lệ với quyền truy cập {AccessFrom}", accessFrom);
                return Result<TokenResultDto>.Failure(ErrorCodes.UNAUTHORIZED_ROLE, ["Bạn không có quyền truy cập hệ thống."]);
            }

            var keycloakIdStr = JwtHelper.ExtractKeycloakIdFromJwt(refreshToken);
            if (Guid.TryParse(keycloakIdStr, out var keycloakId))
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.KeycloakId == keycloakId, cancellationToken);
                if (user != null)
                    await _activityLogService.LogUserActionAsync(user.UserId, ActivityAction.TokenRefreshed);
            }

            return Result<TokenResultDto>.Success(tokenDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi không xác định khi làm mới token");
            return Result<TokenResultDto>.Failure(ErrorCodes.SERVER_ERROR, ["Đã xảy ra lỗi hệ thống. Vui lòng đăng nhập lại."]);
        }
    }
}
