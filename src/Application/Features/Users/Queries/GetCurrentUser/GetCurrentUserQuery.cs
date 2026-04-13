using MediatR;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Users.Queries.GetCurrentUser;

/// <summary>
/// Lấy thông tin user hiện tại từ JWT (sub) và roles trong DB.
/// </summary>
public class GetCurrentUserQuery : IRequest<Result<CurrentUserDto>>
{
}
