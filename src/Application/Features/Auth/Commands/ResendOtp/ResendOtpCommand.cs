using MediatR;
using Shop_Cam_BE.Application.Common.Models;

namespace Shop_Cam_BE.Application.Features.Auth.Commands.ResendOtp;

/// <summary>
/// Gửi lại OTP (chỉ luồng quên mật khẩu); có cooldown và giới hạn theo Redis.
/// </summary>
public class ResendOtpCommand : IRequest<Result<Unit>>
{
    public string? Username { get; set; }
    public string? AccessFrom { get; set; }
    public bool IsForgotPassword { get; set; }
}
