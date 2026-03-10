namespace Shop_Cam_BE.Domain.Entities;

public class ProductCategory
{
    public Guid ProductCategoryId { get; set; }
    public string Name { get; set; } = default!;
    public string? Slug { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}

