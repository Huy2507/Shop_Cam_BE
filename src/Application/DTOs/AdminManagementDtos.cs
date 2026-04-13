namespace Shop_Cam_BE.Application.DTOs;

// --- Products ---

public class AdminProductListItemDto
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public decimal? Discount { get; set; }
    public string ImageUrl { get; set; } = default!;
    public bool IsNew { get; set; }
    public bool OutOfStock { get; set; }
    public Guid? ProductCategoryId { get; set; }
    public string? CategoryName { get; set; }
}

public class AdminProductDetailDto
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public decimal? Discount { get; set; }
    public string? Info { get; set; }
    public string? Description { get; set; }
    public string ImageUrl { get; set; } = default!;
    public bool IsNew { get; set; }
    public bool OutOfStock { get; set; }
    public string? Badge { get; set; }
    public Guid? ProductCategoryId { get; set; }
    public string? CategoryName { get; set; }
}

public class AdminUpsertProductDto
{
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public decimal? Discount { get; set; }
    public string? Info { get; set; }
    public string? Description { get; set; }
    public required string ImageUrl { get; set; }
    public bool IsNew { get; set; }
    public bool OutOfStock { get; set; }
    public string? Badge { get; set; }
    public Guid? ProductCategoryId { get; set; }
}

// --- Categories ---

public class AdminCategoryListItemDto
{
    public Guid ProductCategoryId { get; set; }
    public string Name { get; set; } = default!;
    public string? Slug { get; set; }
    public int ProductCount { get; set; }
}

public class AdminCategoryDetailDto
{
    public Guid ProductCategoryId { get; set; }
    public string Name { get; set; } = default!;
    public string? Slug { get; set; }
}

public class AdminUpsertCategoryDto
{
    public required string Name { get; set; }
    public string? Slug { get; set; }
}

// --- Banners ---

public class AdminBannerListItemDto
{
    public Guid HomeBannerId { get; set; }
    public string UrlImg { get; set; } = default!;
    public string? Title { get; set; }
    public string? Link { get; set; }
    public bool IsMain { get; set; }
    public int DisplayOrder { get; set; }
}

public class AdminUpsertBannerDto
{
    public required string UrlImg { get; set; }
    public string? Title { get; set; }
    public string? Link { get; set; }
    public bool IsMain { get; set; }
    public int DisplayOrder { get; set; }
}

// --- Orders (read-only) ---

public class AdminOrderListItemDto
{
    public Guid OrderId { get; set; }
    public string Code { get; set; } = default!;
    public string CustomerName { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string? Email { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AdminOrderDetailDto
{
    public Guid OrderId { get; set; }
    public string Code { get; set; } = default!;
    public string CustomerName { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string? Email { get; set; }
    public string Address { get; set; } = default!;
    public string? Note { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<AdminOrderLineDto> Items { get; set; } = new();
}

public class AdminOrderLineDto
{
    public Guid OrderItemId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
}
