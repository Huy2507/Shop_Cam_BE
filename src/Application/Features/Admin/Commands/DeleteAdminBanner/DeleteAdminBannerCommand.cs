using MediatR;
using Shop_Cam_BE.Application.Common.Models;

namespace Shop_Cam_BE.Application.Features.Admin.Commands.DeleteAdminBanner;

/// <summary>
/// Xóa banner theo id.
/// </summary>
public class DeleteAdminBannerCommand : IRequest<Result<Unit>>
{
    public Guid BannerId { get; set; }
}
