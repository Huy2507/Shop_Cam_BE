using MediatR;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Auth.Commands.VerifyLoginOtp;

public class VerifyLoginOtpCommand : IRequest<Result<TokenResultDto>>
{
    public string? Email { get; set; }
    public string? Otp { get; set; }
    public string? AccessFrom { get; set; }
}
