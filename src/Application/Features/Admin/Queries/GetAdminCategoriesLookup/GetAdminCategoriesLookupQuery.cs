using MediatR;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Admin.Queries.GetAdminCategoriesLookup;

/// <summary>
/// Danh sách rút gọn id + tên danh mục cho dropdown (toàn bộ, không phân trang).
/// </summary>
public class GetAdminCategoriesLookupQuery : IRequest<List<AdminCategoryLookupDto>>
{
}
