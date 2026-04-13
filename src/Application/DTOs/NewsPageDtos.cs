namespace Shop_Cam_BE.Application.DTOs;

/// <summary>Cấu hình hiệu lực cho trang tin (site + gộp tùy chọn user nếu có).</summary>
public class NewsPagePublicDto
{
    public string PageTitle { get; set; } = "Tin tức";

    public string? PageSubtitle { get; set; }

    /// <summary>magazine | grid | list</summary>
    public string Layout { get; set; } = "magazine";

    public int PageSize { get; set; } = 12;

    public bool ShowFeatured { get; set; } = true;

    public int GridColumns { get; set; } = 4;

    /// <summary>Skin giao diện: classic | editorial | minimal | bold | glass</summary>
    public string VisualTemplate { get; set; } = "classic";

    /// <summary>User đã lưu ít nhất một tùy chọn riêng.</summary>
    public bool Personalized { get; set; }
}

/// <summary>Body PUT — chỉ gửi field cần ghi đè; null = giữ nguyên trong JSON đã lưu.</summary>
public class UpsertUserNewsPagePreferenceDto
{
    public string? Layout { get; set; }

    public int? PageSize { get; set; }

    public bool? ShowFeatured { get; set; }

    public int? GridColumns { get; set; }

    public string? VisualTemplate { get; set; }
}
