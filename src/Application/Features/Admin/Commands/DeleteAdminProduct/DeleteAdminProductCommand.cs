using MediatR;
using Shop_Cam_BE.Application.Common.Models;

namespace Shop_Cam_BE.Application.Features.Admin.Commands.DeleteAdminProduct;

/// <summary>
/// Xóa sản phẩm nếu không còn dòng OrderItems tham chiếu.
/// </summary>
public class DeleteAdminProductCommand : IRequest<Result<Unit>>
{
    public Guid ProductId { get; set; }
}
