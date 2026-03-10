using MediatR;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Auth.Commands.VerifyResetCode;

public class VerifyResetCodeQuery : IRequest<Result<VerifyResetCodeResponse>>
{
    public string Email { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string? AccessFrom { get; set; }
}
