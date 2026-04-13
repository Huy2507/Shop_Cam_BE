using MediatR;
using Shop_Cam_BE.Application.Common.Models;

namespace Shop_Cam_BE.Application.Features.Auth.Commands.Logout;

/// <summary>
/// Đăng xuất: ghi log hoạt động nếu xác định được user từ context.
/// </summary>
public class LogoutCommand : IRequest<Result<Unit>>
{
    public string RefreshToken { get; set; } = string.Empty;
}
