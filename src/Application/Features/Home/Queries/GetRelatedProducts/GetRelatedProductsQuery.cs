using MediatR;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Home.Queries.GetRelatedProducts;

/// <summary>Sản phẩm cùng danh mục (trừ SP hiện tại).</summary>
public class GetRelatedProductsQuery : IRequest<List<CatalogProductDto>>
{
    public Guid ProductId { get; set; }
    public int Take { get; set; } = 5;
}
