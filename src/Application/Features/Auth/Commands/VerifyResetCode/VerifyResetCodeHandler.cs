using MediatR;
using Microsoft.Extensions.Logging;
using Shop_Cam_BE.Application.Common.Constants;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Auth.Commands.VerifyResetCode;

public class VerifyResetCodeHandler : IRequestHandler<VerifyResetCodeQuery, Result<VerifyResetCodeResponse>>
{
    private readonly IRedisService _redisService;
    private readonly IKeycloakService _keycloak;
    private readonly ILogger<VerifyResetCodeHandler> _logger;

    public VerifyResetCodeHandler(
        IRedisService redisService,
        ILogger<VerifyResetCodeHandler> logger,
        IKeycloakService keycloak)
    {
        _redisService = redisService;
        _logger = logger;
        _keycloak = keycloak;
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
            return Result<VerifyResetCodeResponse>.Failure(ErrorCodes.INVALID_OTP, "Mã xác nhận không hợp lệ hoặc đã hết hạn.");
        }

        if (savedCode != request.Code)
        {
            _logger.LogWarning("Mã OTP không đúng cho {Email} ({AccessFrom})", email, accessFrom);
            return Result<VerifyResetCodeResponse>.Failure(ErrorCodes.INVALID_OTP, "Mã xác nhận không chính xác.");
        }

        var userResult = await _keycloak.GetUserIdAndRolesByEmailAsync(email);
        if (!userResult.Succeeded || userResult.Value!.UserId == Guid.Empty)
        {
            _logger.LogWarning("Không tìm thấy người dùng với email {Email} ({AccessFrom})", email, accessFrom);
            return Result<VerifyResetCodeResponse>.Failure(ErrorCodes.USER_NOT_FOUND, "Không tìm thấy người dùng với email này.");
        }

        _logger.LogInformation("Xác thực mã OTP thành công cho {Email} ({AccessFrom})", email, accessFrom);
        await _redisService.SetAsync(redisKey, savedCode, TimeSpan.FromMinutes(3));
        await _redisService.RemoveAsync(attemptsKey);

        return Result<VerifyResetCodeResponse>.Success(new VerifyResetCodeResponse
        {
            IsValid = true,
            UserId = userResult.Value!.UserId
        });
    }
}
