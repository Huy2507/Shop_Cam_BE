using MediatR;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Admin.Queries.GetAdminBannersPaged;

/// <summary>
/// Danh sách banner admin có phân trang.
/// </summary>
public class GetAdminBannersPagedQuery : IRequest<PagedResult<AdminBannerListItemDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Search { get; set; }
}
