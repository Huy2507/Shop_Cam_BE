using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shop_Cam_BE.Application.Common.Constants;
using Shop_Cam_BE.Application.Common.Extensions;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Domain.Enums;

namespace Shop_Cam_BE.Application.Features.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<Unit>>
{
    private readonly IKeycloakService _keycloakService;
    private readonly ILogger<LogoutCommandHandler> _logger;
    private readonly IApplicationDbContext _context;
    private readonly IActivityLogService _activityLogService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LogoutCommandHandler(
        IKeycloakService keycloakService,
        ILogger<LogoutCommandHandler> logger,
        IApplicationDbContext context,
        IActivityLogService activityLogService,
        IHttpContextAccessor httpContextAccessor)
    {
        _keycloakService = keycloakService;
        _logger = logger;
        _context = context;
        _activityLogService = activityLogService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<Unit>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _httpContextAccessor.HttpContext?.GetCurrentUserId(_context);

            var result = await _keycloakService.LogoutUserAsync(request.RefreshToken);
            if (!result)
            {
                _logger.LogWarning("Logout failed for refresh token");
                return Result<Unit>.Failure(ErrorCodes.LOGOUT_FAILED, ["Đăng xuất thất bại"]);
            }

            if (userId.HasValue)
                await _activityLogService.LogUserActionAsync(userId.Value, ActivityAction.UserLoggedOut);

            _logger.LogInformation("User logged out successfully");
            return Result<Unit>.Success(Unit.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during logout");
            return Result<Unit>.Failure(ErrorCodes.SERVER_ERROR, ["Lỗi hệ thống"]);
        }
    }
}
