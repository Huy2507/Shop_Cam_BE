using MediatR;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Auth.Commands.LoginForDashboard;

public class LoginForDashboardCommand : IRequest<Result<LoginResultDto>>
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}
