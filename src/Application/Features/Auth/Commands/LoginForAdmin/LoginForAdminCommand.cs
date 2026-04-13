using MediatR;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Auth.Commands.LoginForAdmin;

/// <summary>
/// Đăng nhập khu quản trị: username/email + mật khẩu cục bộ.
/// </summary>
public class LoginForAdminCommand : IRequest<Result<TokenResultDto>>
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}
