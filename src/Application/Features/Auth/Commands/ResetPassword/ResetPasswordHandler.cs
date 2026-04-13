using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shop_Cam_BE.Application.Common.Constants;
using Shop_Cam_BE.Application.Common.Extensions;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Domain.Entities;
using Shop_Cam_BE.Domain.Enums;

namespace Shop_Cam_BE.Application.Features.Auth.Commands.ResetPassword;

/// <summary>
/// Xác thực OTP Redis, hash mật khẩu mới và xóa key OTP.
/// </summary>
public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, Result<Unit>>
{
    private readonly IRedisService _redisService;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ILogger<ResetPasswordHandler> _logger;
    private readonly IApplicationDbContext _context;
    private readonly IActivityLogService _activityLogService;

    public ResetPasswordHandler(
        IRedisService redisService,
        IPasswordHasher<User> passwordHasher,
        ILogger<ResetPasswordHandler> logger,
        IApplicationDbContext context,
        IActivityLogService activityLogService)
    {
        _redisService = redisService;
        _passwordHasher = passwordHasher;
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
            return Result<Unit>.Failure(ErrorCodes.PASSWORD_MISMATCH);
        }

        var redisKey = $"otp:forgot:{accessFrom}:{email}";
        var savedCode = await _redisService.GetAsync(redisKey);

        if (string.IsNullOrEmpty(savedCode) || savedCode != dto.Code)
        {
            _logger.LogWarning("Mã OTP không hợp lệ hoặc đã hết hạn cho {Email} ({AccessFrom})", email, accessFrom);
            return Result<Unit>.Failure(ErrorCodes.INVALID_OTP);
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("Không tìm thấy người dùng với email {Email} ({AccessFrom})", email, accessFrom);
            return Result<Unit>.Failure(ErrorCodes.USER_NOT_FOUND);
        }

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        await _redisService.RemoveAsync(redisKey);

        await _activityLogService.LogUserActionAsync(user.UserId, ActivityAction.ChangePassword);

        _logger.LogInformation("Đổi mật khẩu thành công cho {Email} ({AccessFrom})", email, accessFrom);
        return Result<Unit>.Success(Unit.Value);
    }
}
