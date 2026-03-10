namespace Shop_Cam_BE.Domain.Entities;

public class HomeBanner
{
    public Guid HomeBannerId { get; set; }
    public string UrlImg { get; set; } = default!;
    public string? Title { get; set; }
    public string? Link { get; set; }
    public bool IsMain { get; set; }
    public int DisplayOrder { get; set; }
}

