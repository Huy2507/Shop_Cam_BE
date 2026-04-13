using MediatR;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Auth.Commands.ResetPassword;

/// <summary>
/// Đặt mật khẩu mới sau khi OTP trong Redis khớp với email/accessFrom.
/// </summary>
public class ResetPasswordCommand : IRequest<Result<Unit>>
{
    public ResetPasswordDto Dto { get; set; } = default!;
}
