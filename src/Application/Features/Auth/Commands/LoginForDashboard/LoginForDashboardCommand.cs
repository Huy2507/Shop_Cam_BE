using MediatR;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Auth.Commands.LoginForDashboard;

/// <summary>
/// Đăng nhập storefront (dashboard): username/email + mật khẩu; quyền theo RoleAccess:dashboard.
/// </summary>
public class LoginForDashboardCommand : IRequest<Result<TokenResultDto>>
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}
