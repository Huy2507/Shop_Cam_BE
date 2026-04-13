using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shop_Cam_BE.Application.Common.Constants;
using Shop_Cam_BE.Application.Common.Helpers;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.Common.Models;

namespace Shop_Cam_BE.Application.Features.Auth.Commands.ForgotPassword;

/// <summary>
/// Sinh OTP, lưu Redis và gửi email khi user tồn tại và có quyền theo RoleAccess:accessFrom.
/// </summary>
public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, Result<Unit>>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IRedisService _redisService;
    private readonly ILogger<ForgotPasswordHandler> _logger;
    private readonly IConfiguration _config;

    public ForgotPasswordHandler(
        IApplicationDbContext context,
        IEmailService emailService,
        IRedisService redisService,
        ILogger<ForgotPasswordHandler> logger,
        IConfiguration config)
    {
        _context = context;
        _emailService = emailService;
        _redisService = redisService;
        _logger = logger;
        _config = config;
    }

    public async Task<Result<Unit>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var email = request.Dto.Email.Trim();
            var accessFrom = request.Dto.AccessFrom?.Trim().ToLower() ?? "app";

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("Email {Email} không tồn tại trong DB", email);
                return Result<Unit>.Failure(ErrorCodes.USER_NOT_FOUND);
            }

            var allowedRoles = _config.GetSection($"RoleAccess:{accessFrom}").Get<string[]>() ?? Array.Empty<string>();
            if (allowedRoles.Length == 0)
            {
                _logger.LogWarning("Không tìm thấy cấu hình RoleAccess cho accessFrom={AccessFrom}", accessFrom);
                return Result<Unit>.Failure(ErrorCodes.FORBIDDEN);
            }

            if (!await UserRoleChecker.UserHasAnyRoleNameAsync(_context, user.UserId, allowedRoles, cancellationToken))
            {
                _logger.LogWarning("Email {Email} không có quyền truy cập {AccessFrom}", email, accessFrom);
                return Result<Unit>.Failure(ErrorCodes.UNAUTHORIZED_ROLE);
            }

            var otp = new Random().Next(100000, 999999).ToString();
            var redisKey = $"otp:forgot:{accessFrom}:{email}";
            var attemptsKey = $"otp:attempts:forgot:{accessFrom}:{email}";

            await _redisService.SetAsync(redisKey, otp, TimeSpan.FromMinutes(3));
            await _redisService.RemoveAsync(attemptsKey);

            var subject = "Mã xác nhận đặt lại mật khẩu";
            var body = $"Mã xác nhận của bạn là: {otp}. Mã này sẽ hết hạn sau 3 phút.";

            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailService.SendEmailAsync(email, subject, body);
                    _logger.LogInformation("Đã gửi OTP đặt lại mật khẩu cho {Email} ({AccessFrom})", email, accessFrom);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi gửi email OTP đặt lại mật khẩu cho {Email}", email);
                }
            });

            return Result<Unit>.Success(Unit.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi trong ForgotPasswordHandler cho {Email}", request.Dto.Email);
            return Result<Unit>.Failure(ErrorCodes.SERVER_ERROR);
        }
    }
}
