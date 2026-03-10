namespace Shop_Cam_BE.Domain.Entities;

public class NewsArticle
{
    public Guid NewsArticleId { get; set; }
    public string Title { get; set; } = default!;
    public string ImageUrl { get; set; } = default!;
    public string? Excerpt { get; set; }
    public string? Link { get; set; }
    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
}

