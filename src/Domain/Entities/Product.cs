namespace Shop_Cam_BE.Domain.Entities;

public class Product
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public decimal? Discount { get; set; }
    public string? Info { get; set; }

    /// <summary>Mô tả chi tiết hiển thị trên trang sản phẩm (PDP).</summary>
    public string? Description { get; set; }

    public string ImageUrl { get; set; } = default!;
    public bool IsNew { get; set; }
    public bool OutOfStock { get; set; }
    public string? Badge { get; set; }

    public Guid? ProductCategoryId { get; set; }
    public ProductCategory? Category { get; set; }

    public ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
}

