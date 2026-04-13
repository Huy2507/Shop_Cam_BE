using Shop_Cam_BE.Domain.Common;

namespace Shop_Cam_BE.Domain.Entities;

public class ProductCategory : IAuditableSoftDeletable
{
    public Guid ProductCategoryId { get; set; }
    public string Name { get; set; } = default!;
    public string? Slug { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();

    public DateTimeOffset CreatedTime { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public DateTimeOffset UpdatedTime { get; set; }
    public Guid? UpdatedByUserId { get; set; }
    public bool IsActive { get; set; } = true;
}
