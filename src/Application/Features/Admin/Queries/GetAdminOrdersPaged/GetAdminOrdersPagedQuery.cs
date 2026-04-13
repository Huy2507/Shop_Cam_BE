using MediatR;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Admin.Queries.GetAdminOrdersPaged;

/// <summary>
/// Danh sách đơn hàng admin có phân trang (tùy lọc trạng thái/tìm).
/// </summary>
public class GetAdminOrdersPagedQuery : IRequest<PagedResult<AdminOrderListItemDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    /// <summary>Tìm theo mã đơn, SĐT, tên khách, email.</summary>
    public string? Search { get; set; }
}
