using MediatR;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Admin.Queries.GetAdminProductById;

/// <summary>
/// Chi tiết một sản phẩm để form sửa (null nếu không tồn tại).
/// </summary>
public class GetAdminProductByIdQuery : IRequest<AdminProductDetailDto?>
{
    public Guid ProductId { get; set; }
}
