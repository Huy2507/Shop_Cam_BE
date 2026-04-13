using MediatR;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Admin.Queries.GetAdminBannerById;

/// <summary>
/// Chi tiết một banner theo id (để form sửa).
/// </summary>
public class GetAdminBannerByIdQuery : IRequest<AdminBannerListItemDto?>
{
    public Guid BannerId { get; set; }
}
