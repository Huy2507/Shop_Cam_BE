using MediatR;
using Shop_Cam_BE.Application.Common.Models;

namespace Shop_Cam_BE.Application.Features.Admin.Commands.DeleteAdminCategory;

/// <summary>
/// Xóa danh mục khi không còn sản phẩm thuộc danh mục đó.
/// </summary>
public class DeleteAdminCategoryCommand : IRequest<Result<Unit>>
{
    public Guid CategoryId { get; set; }
}
