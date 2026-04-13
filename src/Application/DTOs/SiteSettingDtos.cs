namespace Shop_Cam_BE.Application.DTOs;

/// <summary>Payload cấu hình giao diện + tích hợp (storefront đọc, không cần đăng nhập).</summary>
public class PublicUiConfigDto
{
    public string ThemePrimaryColor { get; set; } = "#dc2626";
    public string ThemeAccentColor { get; set; } = "#f59e0b";
    public string? LogoUrl { get; set; }
    public string? FaviconUrl { get; set; }
    public string HeaderBackground { get; set; } = "transparent";
    public string? HeroTagline { get; set; }

    /// <summary>ID Zalo Official Account (số) — bật widget SDK.</summary>
    public string? ZaloOaId { get; set; }

    /// <summary>Link chat khi không dùng OA (vd https://zalo.me/84901234567).</summary>
    public string? ZaloChatUrl { get; set; }

    /// <summary>Lời chào widget Zalo.</summary>
    public string? ZaloWelcomeMessage { get; set; }

    /// <summary>Trang tin tức: tiêu đề, layout, phân trang (đã gộp tùy user nếu đăng nhập).</summary>
    public NewsPagePublicDto NewsPage { get; set; } = new();

    /// <summary>Trang chủ: bật/tắt khối, thứ tự, tab SP mặc định, promo giữa trang.</summary>
    public HomePagePublicDto HomePage { get; set; } = new();
}

public class SiteSettingItemDto
{
    public string Group { get; set; } = default!;
    public string Key { get; set; } = default!;
    public string ValueJson { get; set; } = "{}";
    public string? Description { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class UpsertSiteSettingItemDto
{
    public required string Group { get; set; }
    public required string Key { get; set; }
    public required string ValueJson { get; set; }
}

public class AdminSummaryDto
{
    public int ProductsCount { get; set; }
    public int OrdersCount { get; set; }
    public int UsersCount { get; set; }
    public int ReviewsCount { get; set; }
    public int CategoriesCount { get; set; }
    public int NewsCount { get; set; }
}
