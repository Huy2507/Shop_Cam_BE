using MediatR;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Home.Queries.GetProductById;

/// <summary>Lấy một sản phẩm theo Id cho trang chi tiết.</summary>
public class GetProductByIdQuery : IRequest<ProductDetailDto?>
{
    public Guid ProductId { get; set; }
}
