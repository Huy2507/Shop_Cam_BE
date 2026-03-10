using MediatR;
using Shop_Cam_BE.Application.Common.Models;

namespace Shop_Cam_BE.Application.Features.Auth.Commands.Logout;

public class LogoutCommand : IRequest<Result<Unit>>
{
    public string RefreshToken { get; set; } = string.Empty;
}
