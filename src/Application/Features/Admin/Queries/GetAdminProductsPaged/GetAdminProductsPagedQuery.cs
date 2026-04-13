using MediatR;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Admin.Queries.GetAdminProductsPaged;

/// <summary>
/// Danh sách sản phẩm admin có phân trang, tìm theo tên và lọc theo danh mục.
/// </summary>
public class GetAdminProductsPagedQuery : IRequest<PagedResult<AdminProductListItemDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Search { get; set; }
    public Guid? CategoryId { get; set; }
}
