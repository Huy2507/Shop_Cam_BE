using MediatR;
using Shop_Cam_BE.Domain.Entities;

namespace Shop_Cam_BE.Application.Features.Home.Queries.GetProducts;

/// <summary>
/// Query lấy danh sách sản phẩm theo filter hiển thị trên trang chủ.
/// - "best": bán chạy nhất (mặc định)
/// - "hot": sản phẩm có giảm giá
/// - "combo": tạm thời tận dụng logic Skip(2) giống FE mock ban đầu
/// </summary>
public class GetProductsQuery : IRequest<List<Product>>
{
    /// <summary>
    /// Giá trị filter từ query string (best | hot | combo).
    /// </summary>
    public string? Filter { get; set; }
}

