using MediatR;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Admin.Queries.GetAdminCategoryById;

/// <summary>
/// Chi tiết một danh mục theo id.
/// </summary>
public class GetAdminCategoryByIdQuery : IRequest<AdminCategoryDetailDto?>
{
    public Guid CategoryId { get; set; }
}
