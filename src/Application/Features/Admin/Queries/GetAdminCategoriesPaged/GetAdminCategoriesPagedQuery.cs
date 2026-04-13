using MediatR;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Admin.Queries.GetAdminCategoriesPaged;

/// <summary>
/// Danh sách danh mục có phân trang và tìm theo tên.
/// </summary>
public class GetAdminCategoriesPagedQuery : IRequest<PagedResult<AdminCategoryListItemDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Search { get; set; }
}
