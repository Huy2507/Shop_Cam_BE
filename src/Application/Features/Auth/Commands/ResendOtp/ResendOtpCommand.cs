using MediatR;
using Shop_Cam_BE.Application.Common.Models;

namespace Shop_Cam_BE.Application.Features.Auth.Commands.ResendOtp;

public class ResendOtpCommand : IRequest<Result<Unit>>
{
    public string? Username { get; set; }
    public string? AccessFrom { get; set; }
    public bool IsForgotPassword { get; set; }
}
