using MediatR;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Admin.Commands.CreateAdminBanner;

/// <summary>
/// Tạo banner (ảnh URL, thứ tự, trạng thái active).
/// </summary>
public class CreateAdminBannerCommand : IRequest<Result<Guid>>
{
    public required AdminUpsertBannerDto Dto { get; set; }
}
