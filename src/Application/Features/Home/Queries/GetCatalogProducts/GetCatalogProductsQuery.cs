using MediatR;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Home.Queries.GetCatalogProducts;

/// <summary>
/// Danh mục / tìm kiếm sản phẩm có phân trang, lọc, sắp xếp.
/// </summary>
public class GetCatalogProductsQuery : IRequest<CatalogProductsResult>
{
    public string? Search { get; set; }
    public string? CategoryName { get; set; }
    /// <summary>price_asc | price_desc | newest | name</summary>
    public string? Sort { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }

    /// <summary>best | hot | combo — giống bộ lọc trang chủ.</summary>
    public string? HomeTab { get; set; }
}
