using Shop_Cam_BE.Application.DTOs;
using Shop_Cam_BE.Domain.Entities;

namespace Shop_Cam_BE.Application.Common.Extensions;

/// <summary>Filter, sort và projection Product cho Home/catalog (EF-translatable).</summary>
public static class ProductStorefrontQueryExtensions
{
    /// <summary>hot = có discount; combo = Skip(2); còn lại không lọc thêm.</summary>
    public static IQueryable<Product> ApplyStorefrontHomeTabOrFilter(this IQueryable<Product> query, string? tabOrFilter)
    {
        var key = tabOrFilter?.Trim().ToLowerInvariant();
        if (key == "hot")
            return query.Where(p => p.Discount != null && p.Discount > 0);
        if (key == "combo")
            return query.Skip(2);
        return query;
    }

    /// <summary>Lọc theo tên danh mục (khớp đúng).</summary>
    public static IQueryable<Product> ApplyStorefrontCategoryName(this IQueryable<Product> query, string? categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
            return query;
        var cat = categoryName.Trim();
        return query.Where(p => p.Category != null && p.Category.Name == cat);
    }

    /// <summary>Tìm theo tên (Contains).</summary>
    public static IQueryable<Product> ApplyStorefrontNameSearch(this IQueryable<Product> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return query;
        var s = search.Trim();
        return query.Where(p => p.Name.Contains(s));
    }

    /// <summary>Lọc theo giá sau giảm &gt;= min.</summary>
    public static IQueryable<Product> ApplyStorefrontMinEffectivePrice(this IQueryable<Product> query, decimal? minPrice)
    {
        if (!minPrice.HasValue)
            return query;
        var min = minPrice.Value;
        return query.Where(p =>
            (p.Discount.HasValue && p.Discount > 0 ? p.Price - p.Discount.Value : p.Price) >= min);
    }

    /// <summary>Lọc theo giá sau giảm &lt;= max.</summary>
    public static IQueryable<Product> ApplyStorefrontMaxEffectivePrice(this IQueryable<Product> query, decimal? maxPrice)
    {
        if (!maxPrice.HasValue)
            return query;
        var max = maxPrice.Value;
        return query.Where(p =>
            (p.Discount.HasValue && p.Discount > 0 ? p.Price - p.Discount.Value : p.Price) <= max);
    }

    /// <summary>price_asc | price_desc | name | mặc định newest.</summary>
    public static IQueryable<Product> ApplyStorefrontCatalogSort(this IQueryable<Product> query, string? sort)
    {
        return (sort ?? "newest").ToLowerInvariant() switch
        {
            "price_asc" => query.OrderBy(p =>
                p.Discount.HasValue && p.Discount > 0 ? p.Price - p.Discount.Value : p.Price),
            "price_desc" => query.OrderByDescending(p =>
                p.Discount.HasValue && p.Discount > 0 ? p.Price - p.Discount.Value : p.Price),
            "name" => query.OrderBy(p => p.Name),
            _ => query.OrderByDescending(p => p.IsNew).ThenByDescending(p => p.ProductId),
        };
    }

    /// <summary>Projection sang DTO dòng catalog.</summary>
    public static IQueryable<CatalogProductDto> SelectAsCatalogProductDto(this IQueryable<Product> query) =>
        query.Select(p => new CatalogProductDto
        {
            Id = p.ProductId,
            Name = p.Name,
            Price = p.Price,
            Discount = p.Discount ?? 0m,
            Info = p.Info ?? string.Empty,
            ImageUrl = p.ImageUrl,
            IsNew = p.IsNew,
            OutOfStock = p.OutOfStock,
            Badge = p.Badge ?? string.Empty,
            CategoryName = p.Category != null ? p.Category.Name : null
        });

    /// <summary>Projection sang DTO PDP.</summary>
    public static IQueryable<ProductDetailDto> SelectAsProductDetailDto(this IQueryable<Product> query) =>
        query.Select(p => new ProductDetailDto
        {
            Id = p.ProductId,
            Name = p.Name,
            Price = p.Price,
            Discount = p.Discount,
            Info = p.Info,
            Description = p.Description,
            ImageUrl = p.ImageUrl,
            IsNew = p.IsNew,
            OutOfStock = p.OutOfStock,
            Badge = p.Badge,
            CategoryName = p.Category != null ? p.Category.Name : null
        });
}
