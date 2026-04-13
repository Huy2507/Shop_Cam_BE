using Shop_Cam_BE.Domain.Entities;

namespace Shop_Cam_BE.Application.DTOs;

/// <summary>Chi tiết sản phẩm cho trang PDP.</summary>
public class ProductDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public decimal? Discount { get; set; }
    public string? Info { get; set; }
    public string? Description { get; set; }
    public string ImageUrl { get; set; } = default!;
    public bool IsNew { get; set; }
    public bool OutOfStock { get; set; }
    public string? Badge { get; set; }
    public string? CategoryName { get; set; }
}

/// <summary>Một dòng trong danh mục / tìm kiếm.</summary>
public class CatalogProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
    public string Info { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = default!;
    public bool IsNew { get; set; }
    public bool OutOfStock { get; set; }
    public string Badge { get; set; } = string.Empty;
    public string? CategoryName { get; set; }
}

/// <summary>Kết quả phân trang catalog.</summary>
public class CatalogProductsResult
{
    public List<CatalogProductDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}

/// <summary>Chi tiết tin tức.</summary>
public class NewsDetailDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string ImageUrl { get; set; } = default!;
    public string? Excerpt { get; set; }
    public string? Body { get; set; }
    public DateTime PublishedAt { get; set; }
}

/// <summary>Danh sách tin phân trang (GET /api/home/news).</summary>
public class NewsFeedResult
{
    public List<NewsArticle> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}

/// <summary>Một đánh giá hiển thị trên PDP.</summary>
public class ProductReviewDto
{
    public Guid Id { get; set; }
    public string AuthorName { get; set; } = default!;
    public byte Rating { get; set; }
    public string Comment { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}

/// <summary>Danh sách đánh giá + điểm trung bình.</summary>
public class ProductReviewsResult
{
    public List<ProductReviewDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public double? AverageRating { get; set; }
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}
