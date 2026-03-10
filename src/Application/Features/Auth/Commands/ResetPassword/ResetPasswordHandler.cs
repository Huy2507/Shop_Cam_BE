using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shop_Cam_BE.Application.Common.Constants;
using Shop_Cam_BE.Application.Common.Extensions;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Domain.Enums;

namespace Shop_Cam_BE.Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, Result<Unit>>
{
    private readonly IRedisService _redisService;
    private readonly IKeycloakService _keycloakService;
    private readonly ILogger<ResetPasswordHandler> _logger;
    private readonly IApplicationDbContext _context;
    private readonly IActivityLogService _activityLogService;

    public ResetPasswordHandler(
        IRedisService redisService,
        IKeycloakService keycloakService,
        ILogger<ResetPasswordHandler> logger,
        IApplicationDbContext context,
        IActivityLogService activityLogService)
    {
        _redisService = redisService;
        _keycloakService = keycloakService;
        _logger = logger;
        _context = context;
        _activityLogService = activityLogService;
    }

    public async Task<Result<Unit>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var email = dto.Email;
        var accessFrom = dto.AccessFrom?.Trim().ToLower() ?? "app";

        if (dto.NewPassword != dto.ConfirmPassword)
        {
            _logger.LogWarning("Mật khẩu xác nhận không khớp cho {Email}", email);
            return Result<Unit>.Failure(ErrorCodes.PASSWORD_MISMATCH, "Mật khẩu xác nhận không khớp.");
        }

        var redisKey = $"otp:forgot:{accessFrom}:{email}";
        var savedCode = await _redisService.GetAsync(redisKey);

        if (string.IsNullOrEmpty(savedCode) || savedCode != dto.Code)
        {
            _logger.LogWarning("Mã OTP không hợp lệ hoặc đã hết hạn cho {Email} ({AccessFrom})", email, accessFrom);
            return Result<Unit>.Failure(ErrorCodes.INVALID_OTP, "Mã xác nhận không hợp lệ hoặc đã hết hạn.");
        }

        var userInfoResult = await _keycloakService.GetUserIdAndRolesByEmailAsync(email);
        if (!userInfoResult.Succeeded || userInfoResult.Value!.UserId == Guid.Empty)
        {
            _logger.LogWarning("Không tìm thấy người dùng với email {Email} ({AccessFrom})", email, accessFrom);
            return Result<Unit>.Failure(ErrorCodes.USER_NOT_FOUND, "Không tìm thấy người dùng với email này.");
        }

        var updateResult = await _keycloakService.ResetPasswordAsync(userInfoResult.Value!.UserId, dto.NewPassword);
        if (!updateResult.Succeeded)
        {
            _logger.LogError("Đổi mật khẩu thất bại cho {Email} ({AccessFrom})", email, accessFrom);
            return Result<Unit>.Failure(ErrorCodes.OPERATION_FAILED, "Đổi mật khẩu thất bại. Vui lòng thử lại.");
        }

        await _redisService.RemoveAsync(redisKey);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.KeycloakId == userInfoResult.Value.UserId, cancellationToken);
        if (user != null)
            await _activityLogService.LogUserActionAsync(user.UserId, ActivityAction.ChangePassword);

        _logger.LogInformation("Đổi mật khẩu thành công cho {Email} ({AccessFrom})", email, accessFrom);
        return Result<Unit>.Success(Unit.Value);
    }
}
