using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shop_Cam_BE.Application.Common.Constants;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Auth.Commands.VerifyResetCode;

/// <summary>
/// So khớp OTP trong Redis với user theo email; trả UserId khi hợp lệ.
/// </summary>
public class VerifyResetCodeHandler : IRequestHandler<VerifyResetCodeQuery, Result<VerifyResetCodeResponse>>
{
    private readonly IRedisService _redisService;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<VerifyResetCodeHandler> _logger;

    public VerifyResetCodeHandler(
        IRedisService redisService,
        IApplicationDbContext context,
        ILogger<VerifyResetCodeHandler> logger)
    {
        _redisService = redisService;
        _context = context;
        _logger = logger;
    }

    public async Task<Result<VerifyResetCodeResponse>> Handle(VerifyResetCodeQuery request, CancellationToken cancellationToken)
    {
        var email = request.Email;
        var accessFrom = request.AccessFrom?.Trim().ToLower() ?? "app";

        var redisKey = $"otp:forgot:{accessFrom}:{email}";
        var attemptsKey = $"otp:attempts:forgot:{accessFrom}:{email}";
        var savedCode = await _redisService.GetAsync(redisKey);

        if (string.IsNullOrEmpty(savedCode))
        {
            _logger.LogWarning("Không tìm thấy hoặc mã OTP đã hết hạn cho {Email} ({AccessFrom})", email, accessFrom);
            return Result<VerifyResetCodeResponse>.Failure(ErrorCodes.INVALID_OTP);
        }

        if (savedCode != request.Code)
        {
            _logger.LogWarning("Mã OTP không đúng cho {Email} ({AccessFrom})", email, accessFrom);
            return Result<VerifyResetCodeResponse>.Failure(ErrorCodes.INVALID_OTP);
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("Không tìm thấy người dùng với email {Email} ({AccessFrom})", email, accessFrom);
            return Result<VerifyResetCodeResponse>.Failure(ErrorCodes.USER_NOT_FOUND);
        }

        _logger.LogInformation("Xác thực mã OTP thành công cho {Email} ({AccessFrom})", email, accessFrom);
        await _redisService.SetAsync(redisKey, savedCode, TimeSpan.FromMinutes(3));
        await _redisService.RemoveAsync(attemptsKey);

        return Result<VerifyResetCodeResponse>.Success(new VerifyResetCodeResponse
        {
            IsValid = true,
            UserId = user.UserId,
        });
    }
}
