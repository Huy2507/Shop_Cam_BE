using Shop_Cam_BE.Application.DTOs;
using Shop_Cam_BE.Domain.Entities;

namespace Shop_Cam_BE.Application.Common.Mappings;

/// <summary>Map entity/DTO sang JSON camelCase cho HomeController.</summary>
public static class HomeStorefrontResponseMapper
{
    /// <summary>Danh mục menu → JSON.</summary>
    public static object FromCategory(ProductCategory c) => new
    {
        id = c.ProductCategoryId,
        name = c.Name,
        slug = c.Slug
    };

    /// <summary>Banner → JSON.</summary>
    public static object FromBanner(HomeBanner b) => new
    {
        id = b.HomeBannerId,
        urlimg = b.UrlImg,
        title = b.Title,
        link = b.Link
    };

    /// <summary>Sản phẩm trang chủ (discount số).</summary>
    public static object FromProductForHomeList(Product p) => new
    {
        id = p.ProductId,
        name = p.Name,
        price = p.Price,
        discount = p.Discount ?? 0m,
        info = p.Info ?? string.Empty,
        imageUrl = p.ImageUrl,
        isNew = p.IsNew,
        outOfStock = p.OutOfStock,
        badge = p.Badge ?? string.Empty
    };

    /// <summary>Sản phẩm mới (discount nullable).</summary>
    public static object FromProductForNewArrivals(Product p) => new
    {
        id = p.ProductId,
        name = p.Name,
        price = p.Price,
        discount = p.Discount,
        info = p.Info,
        imageUrl = p.ImageUrl,
        isNew = p.IsNew,
        outOfStock = p.OutOfStock,
        badge = p.Badge
    };

    /// <summary>Tin (list).</summary>
    public static object FromNewsArticle(NewsArticle n) => new
    {
        id = n.NewsArticleId,
        title = n.Title,
        imageUrl = n.ImageUrl,
        excerpt = n.Excerpt ?? string.Empty,
        link = n.Link,
        publishedAt = n.PublishedAt
    };

    /// <summary>Tin (chi tiết).</summary>
    public static object FromNewsDetail(NewsDetailDto dto) => new
    {
        id = dto.Id,
        title = dto.Title,
        imageUrl = dto.ImageUrl,
        excerpt = dto.Excerpt ?? string.Empty,
        body = dto.Body ?? string.Empty,
        publishedAt = dto.PublishedAt
    };

    /// <summary>Sản phẩm PDP.</summary>
    public static object FromProductDetail(ProductDetailDto p) => new
    {
        id = p.Id,
        name = p.Name,
        price = p.Price,
        discount = p.Discount ?? 0m,
        info = p.Info ?? string.Empty,
        description = p.Description ?? string.Empty,
        imageUrl = p.ImageUrl,
        isNew = p.IsNew,
        outOfStock = p.OutOfStock,
        badge = p.Badge ?? string.Empty,
        categoryName = p.CategoryName
    };

    /// <summary>Sản phẩm catalog / liên quan.</summary>
    public static object FromCatalogProduct(CatalogProductDto p) => new
    {
        id = p.Id,
        name = p.Name,
        price = p.Price,
        discount = p.Discount,
        info = p.Info,
        imageUrl = p.ImageUrl,
        isNew = p.IsNew,
        outOfStock = p.OutOfStock,
        badge = p.Badge,
        categoryName = p.CategoryName
    };
}
