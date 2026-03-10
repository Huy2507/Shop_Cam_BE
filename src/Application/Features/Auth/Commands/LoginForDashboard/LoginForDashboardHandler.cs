using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shop_Cam_BE.Application.Common.Constants;
using Shop_Cam_BE.Application.Common.Helpers;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Auth.Commands.LoginForDashboard;

public class LoginForDashboardHandler : IRequestHandler<LoginForDashboardCommand, Result<LoginResultDto>>
{
    private readonly IKeycloakService _keycloakService;
    private readonly IEmailService _emailService;
    private readonly IRedisService _redisService;
    private readonly ILogger<LoginForDashboardHandler> _logger;
    private readonly IConfiguration _config;
    private readonly IApplicationDbContext _context;

    public LoginForDashboardHandler(
        IKeycloakService keycloakService,
        IEmailService emailService,
        IRedisService redisService,
        ILogger<LoginForDashboardHandler> logger,
        IConfiguration config,
        IApplicationDbContext context)
    {
        _keycloakService = keycloakService;
        _emailService = emailService;
        _redisService = redisService;
        _logger = logger;
        _config = config;
        _context = context;
    }

    public async Task<Result<LoginResultDto>> Handle(LoginForDashboardCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var username = request.Username;
            var accessFrom = "dashboard";

            _logger.LogInformation("Xử lý đăng nhập Dashboard cho {Username}", username);

            var loginResult = await _keycloakService.LoginAsync(username, request.Password);
            if (!loginResult.Succeeded)
                return Result<LoginResultDto>.Failure(ErrorCodes.INVALID_CREDENTIALS, ["Tài khoản hoặc mật khẩu không đúng."]);

            Guid keycloakId;
            var userInfoResult = await _keycloakService.GetUserInfoByUsernameAsync(username);
            var userInfoResult2 = await _keycloakService.GetUserIdAndRolesByEmailAsync(username);

            if (userInfoResult.Succeeded && !string.IsNullOrWhiteSpace(userInfoResult.Value?.KeycloakId))
                keycloakId = Guid.Parse(userInfoResult.Value.KeycloakId);
            else if (userInfoResult2.Succeeded && userInfoResult2.Value != null && userInfoResult2.Value.UserId != Guid.Empty)
                keycloakId = userInfoResult2.Value.UserId;
            else
                return Result<LoginResultDto>.Failure(ErrorCodes.USER_NOT_FOUND, ["Không thể xác định tài khoản người dùng từ Keycloak."]);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.KeycloakId == keycloakId, cancellationToken);
            if (user == null)
                return Result<LoginResultDto>.Failure(ErrorCodes.USER_NOT_FOUND, ["Tài khoản không tồn tại trong hệ thống."]);
            if (!user.IsActive)
                return Result<LoginResultDto>.Failure(ErrorCodes.ACCOUNT_LOCKED, ["Tài khoản đã bị khóa. Vui lòng liên hệ quản trị viên."]);

            var token = loginResult.Value?.AccessToken;
            var allowedRoles = _config.GetSection($"RoleAccess:{accessFrom}").Get<string[]>() ?? Array.Empty<string>();
            if (string.IsNullOrWhiteSpace(token) || !await _keycloakService.CheckUserHasAnyRoleAsync(token, allowedRoles))
                return Result<LoginResultDto>.Failure(ErrorCodes.UNAUTHORIZED_ROLE, ["Bạn không có quyền truy cập dashboard."]);

            string emailToSendOtp = ValidationUtils.IsValidEmail(username)
                ? username
                : userInfoResult.Value?.Email ?? username;
            if (string.IsNullOrWhiteSpace(emailToSendOtp))
                return Result<LoginResultDto>.Failure(ErrorCodes.USER_NOT_FOUND, ["Không thể xác định email người dùng."]);

            loginResult.Value!.Email = emailToSendOtp;

            var otp = new Random().Next(100000, 999999).ToString();
            var otpKey = $"otp:login:{accessFrom}:{emailToSendOtp}";
            var tempPwdKey = $"temp-login:{accessFrom}:{emailToSendOtp}";
            await _redisService.SetAsync(otpKey, otp, TimeSpan.FromMinutes(3));
            await _redisService.SetAsync(tempPwdKey, request.Password, TimeSpan.FromMinutes(3));

            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailService.SendEmailAsync(emailToSendOtp, "Mã OTP đăng nhập", $"Mã OTP của bạn là: {otp}. Mã này sẽ hết hạn sau 3 phút.");
                    _logger.LogInformation("Đã gửi OTP đến {Email}", emailToSendOtp);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi gửi email OTP đến {Email}", emailToSendOtp);
                }
            });

            return Result<LoginResultDto>.Success(new LoginResultDto { Email = emailToSendOtp });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi hệ thống khi đăng nhập Dashboard cho {Username}", request.Username);
            return Result<LoginResultDto>.Failure(ErrorCodes.SERVER_ERROR, ["Đã xảy ra lỗi hệ thống, vui lòng thử lại sau."]);
        }
    }
}
