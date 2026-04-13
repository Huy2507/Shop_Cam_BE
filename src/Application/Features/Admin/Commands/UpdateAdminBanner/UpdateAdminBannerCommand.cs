using MediatR;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Admin.Commands.UpdateAdminBanner;

/// <summary>
/// Cập nhật banner theo BannerId.
/// </summary>
public class UpdateAdminBannerCommand : IRequest<Result<Unit>>
{
    public Guid BannerId { get; set; }
    public required AdminUpsertBannerDto Dto { get; set; }
}
