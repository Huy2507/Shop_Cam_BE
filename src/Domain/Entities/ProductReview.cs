namespace Shop_Cam_BE.Domain.Entities;

public class ProductReview
{
    public Guid ProductReviewId { get; set; }
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;

    public string AuthorName { get; set; } = default!;
    /// <summary>Số sao 1–5.</summary>
    public byte Rating { get; set; }
    public string Comment { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    /// <summary>Ẩn trên storefront khi false (duyệt sau).</summary>
    public bool IsApproved { get; set; } = true;
}
