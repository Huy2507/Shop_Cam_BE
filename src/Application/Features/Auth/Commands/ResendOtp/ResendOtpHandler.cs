using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shop_Cam_BE.Application.Common.Constants;
using Shop_Cam_BE.Application.Common.Helpers;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.Common.Models;

namespace Shop_Cam_BE.Application.Features.Auth.Commands.ResendOtp;

/// <summary>
/// Resolve email từ username/email, kiểm tra cooldown rồi gửi OTP qua email.
/// </summary>
public class ResendOtpHandler : IRequestHandler<ResendOtpCommand, Result<Unit>>
{
    private const int OtpResendCooldownSeconds = 60;
    private static readonly Random Random = new();

    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IRedisService _redisService;
    private readonly ILogger<ResendOtpHandler> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ResendOtpHandler(
        IApplicationDbContext context,
        IEmailService emailService,
        IRedisService redisService,
        ILogger<ResendOtpHandler> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _emailService = emailService;
        _redisService = redisService;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<Unit>> Handle(ResendOtpCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (!request.IsForgotPassword)
                return Result<Unit>.Failure(ErrorCodes.RESEND_OTP_NOT_SUPPORTED);

            var accessFrom = string.IsNullOrWhiteSpace(request.AccessFrom) ? "app" : request.AccessFrom.Trim().ToLower();
            var username = request.Username ?? _httpContextAccessor.HttpContext?.Request.Cookies["username"];

            if (string.IsNullOrEmpty(username))
                return Result<Unit>.Failure(ErrorCodes.MISSING_USERNAME_OR_EMAIL);

            string email;
            if (ValidationUtils.IsValidEmail(username))
            {
                email = username;
            }
            else
            {
                var user = await AuthUserQueries.FindByUsernameOrEmailAsync(_context, username, cancellationToken);
                if (user == null || string.IsNullOrWhiteSpace(user.Email))
                    return Result<Unit>.Failure(ErrorCodes.USER_NOT_FOUND);
                email = user.Email;
            }

            var context = "forgot";
            var otpKey = $"otp:{context}:{accessFrom}:{email}";
            var attemptsKey = $"otp:attempts:{context}:{accessFrom}:{email}";
            var lastSentKey = $"otp:last_sent:{context}:{accessFrom}:{email}";

            var lastSentStr = await _redisService.GetAsync(lastSentKey);
            if (DateTime.TryParse(lastSentStr, out var lastSentTime))
            {
                var secondsSinceLastSent = (DateTime.UtcNow - lastSentTime).TotalSeconds;
                if (secondsSinceLastSent < OtpResendCooldownSeconds)
                {
                    var waitSeconds = OtpResendCooldownSeconds - (int)secondsSinceLastSent;
                    return Result<Unit>.Failure(ErrorCodes.OTP_RESEND_COOLDOWN, [waitSeconds.ToString()]);
                }
            }

            var newOtp = Random.Next(100000, 999999).ToString();
            await _redisService.SetAsync(otpKey, newOtp, TimeSpan.FromMinutes(3));
            await _redisService.RemoveAsync(attemptsKey);

            var subject = "Mã xác nhận đặt lại mật khẩu";
            var body = $"Bạn đã yêu cầu đặt lại mật khẩu. Mã xác nhận của bạn là: {newOtp}. Mã này sẽ hết hạn sau 3 phút.";

            await _emailService.SendEmailAsync(email, subject, body);
            _logger.LogInformation("Đã gửi lại OTP cho {Email} với context {Context}", email, context);
            await _redisService.SetAsync(lastSentKey, DateTime.UtcNow.ToString("o"), TimeSpan.FromSeconds(OtpResendCooldownSeconds));

            return Result<Unit>.Success(Unit.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi gửi lại OTP.");
            return Result<Unit>.Failure(ErrorCodes.OTP_SEND_FAILED);
        }
    }
}
