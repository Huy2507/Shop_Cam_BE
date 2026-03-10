using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shop_Cam_BE.Application.Common.Constants;
using Shop_Cam_BE.Application.Common.Extensions;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;
using Shop_Cam_BE.Domain.Enums;

namespace Shop_Cam_BE.Application.Features.Auth.Commands.VerifyLoginOtp;

public class VerifyLoginOtpHandler : IRequestHandler<VerifyLoginOtpCommand, Result<TokenResultDto>>
{
    private readonly IRedisService _redisService;
    private readonly IKeycloakService _keycloakService;
    private readonly ILogger<VerifyLoginOtpHandler> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IApplicationDbContext _context;
    private readonly IActivityLogService _activityLogService;

    public VerifyLoginOtpHandler(
        IRedisService redisService,
        IKeycloakService keycloakService,
        ILogger<VerifyLoginOtpHandler> logger,
        IHttpContextAccessor httpContextAccessor,
        IApplicationDbContext context,
        IActivityLogService activityLogService)
    {
        _redisService = redisService;
        _keycloakService = keycloakService;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _context = context;
        _activityLogService = activityLogService;
    }

    public async Task<Result<TokenResultDto>> Handle(VerifyLoginOtpCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var accessFrom = string.IsNullOrWhiteSpace(request.AccessFrom) ? "app" : request.AccessFrom.Trim().ToLower();
            var email = request.Email ?? _httpContextAccessor.HttpContext?.Request.Cookies["email"];

            if (string.IsNullOrWhiteSpace(email))
                return Result<TokenResultDto>.Failure(ErrorCodes.UNAUTHORIZED, ["Không tìm thấy thông tin email. Vui lòng đăng nhập lại."]);

            var redisOtpKey = $"otp:login:{accessFrom}:{email}";
            var redisAttemptsKey = $"otp:attempts:login:{accessFrom}:{email}";
            var currentAttempts = await _redisService.GetAsync<int?>(redisAttemptsKey) ?? 0;

            if (currentAttempts >= 5)
                return Result<TokenResultDto>.Failure(ErrorCodes.TOO_MANY_ATTEMPTS, ["Bạn đã nhập sai quá nhiều lần. Vui lòng yêu cầu mã OTP mới."]);

            var savedOtp = await _redisService.GetAsync(redisOtpKey);
            if (string.IsNullOrWhiteSpace(savedOtp) || savedOtp != request.Otp)
            {
                await _redisService.SetAsync(redisAttemptsKey, currentAttempts + 1, TimeSpan.FromMinutes(5));
                return Result<TokenResultDto>.Failure(ErrorCodes.INVALID_OTP, ["Mã OTP không chính xác."]);
            }

            await _redisService.RemoveAsync(redisOtpKey);
            await _redisService.RemoveAsync(redisAttemptsKey);

            var tempPasswordKey = $"temp-login:{accessFrom}:{email}";
            var tempPassword = await _redisService.GetAsync(tempPasswordKey);
            if (string.IsNullOrWhiteSpace(tempPassword))
                return Result<TokenResultDto>.Failure(ErrorCodes.UNAUTHORIZED, ["Không tìm thấy mật khẩu tạm. Vui lòng đăng nhập lại."]);

            var loginResult = await _keycloakService.LoginAsync(email, tempPassword);
            await _redisService.RemoveAsync(tempPasswordKey);

            if (!loginResult.Succeeded)
                return Result<TokenResultDto>.Failure(ErrorCodes.INVALID_CREDENTIALS, ["Không thể đăng nhập với thông tin đã lưu."]);

            var userInfoResult = await _keycloakService.GetUserIdAndRolesByEmailAsync(email);
            var keycloakId = userInfoResult.Value!.UserId;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.KeycloakId == keycloakId, cancellationToken);
            if (user == null)
                return Result<TokenResultDto>.Failure(ErrorCodes.USER_NOT_FOUND, ["Tài khoản không tồn tại trong hệ thống."]);
            if (!user.IsActive)
                return Result<TokenResultDto>.Failure(ErrorCodes.ACCOUNT_LOCKED, ["Tài khoản đã bị khóa. Vui lòng liên hệ quản trị viên."]);

            await _activityLogService.LogUserActionAsync(user.UserId, ActivityAction.UserLoggedIn);
            return loginResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi xác thực OTP.");
            return Result<TokenResultDto>.Failure(ErrorCodes.OPERATION_FAILED, ["Xảy ra lỗi khi xác thực mã OTP. Vui lòng thử lại."]);
        }
    }
}
