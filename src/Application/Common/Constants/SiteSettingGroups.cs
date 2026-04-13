namespace Shop_Cam_BE.Application.Common.Constants;

/// <summary>Nhóm cấu hình và khóa được phép (whitelist) để tránh ghi tùy ý.</summary>
public static class SiteSettingGroups
{
    public const string Ui = "UI";

    /// <summary>Zalo OA, link chat dự phòng, v.v. (khớp biến VITE_* trên FE).</summary>
    public const string Integrations = "Integrations";

    /// <summary>Tiêu đề, layout mặc định, phân trang trang tin tức storefront.</summary>
    public const string NewsPage = "NewsPage";

    /// <summary>Bố cục và khối hiển thị trang chủ.</summary>
    public const string HomePage = "HomePage";

    public static readonly HashSet<string> UiKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "theme_primary_color",
        "theme_accent_color",
        "logo_url",
        "favicon_url",
        "header_background",
        "hero_tagline",
    };

    public static readonly HashSet<string> IntegrationsKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "zalo_oa_id",
        "zalo_chat_url",
        "zalo_welcome_message",
    };

    public static readonly HashSet<string> NewsPageKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "news_page_title",
        "news_page_subtitle",
        "news_default_layout",
        "news_page_size",
        "news_show_featured",
        "news_grid_columns",
        "news_visual_template",
    };

    public static readonly HashSet<string> HomePageKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "home_show_banner_block",
        "home_show_promo_sidebar",
        "home_show_mid_promo",
        "home_show_product_tabs",
        "home_show_new_arrivals",
        "home_show_news",
        "home_section_order",
        "home_default_product_tab",
        "home_new_arrivals_title",
        "home_news_take",
        "home_mid_promo_json",
    };

    public static bool IsAllowedKey(string group, string key)
    {
        if (group.Equals(Ui, StringComparison.OrdinalIgnoreCase))
            return UiKeys.Contains(key);
        if (group.Equals(Integrations, StringComparison.OrdinalIgnoreCase))
            return IntegrationsKeys.Contains(key);
        if (group.Equals(NewsPage, StringComparison.OrdinalIgnoreCase))
            return NewsPageKeys.Contains(key);
        if (group.Equals(HomePage, StringComparison.OrdinalIgnoreCase))
            return HomePageKeys.Contains(key);
        return false;
    }
}
