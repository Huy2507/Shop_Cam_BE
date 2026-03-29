namespace Shop_Cam_BE.Domain.Entities;

public class NewsArticle
{
    public Guid NewsArticleId { get; set; }
    public string Title { get; set; } = default!;
    public string ImageUrl { get; set; } = default!;
    public string? Excerpt { get; set; }

    /// <summary>Nội dung bài viết đầy đủ (trang chi tiết tin).</summary>
    public string? Body { get; set; }

    public string? Link { get; set; }
    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
}

