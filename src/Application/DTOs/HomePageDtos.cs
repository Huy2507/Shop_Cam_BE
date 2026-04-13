namespace Shop_Cam_BE.Application.DTOs;

/// <summary>Cấu hình hiển thị trang chủ storefront (SiteSettings nhóm HomePage).</summary>
public class HomePagePublicDto
{
    public bool ShowBannerBlock { get; set; } = true;

    public bool ShowPromoSidebar { get; set; } = true;

    public bool ShowMidPromo { get; set; } = true;

    public bool ShowProductTabs { get; set; } = true;

    public bool ShowNewArrivals { get; set; } = true;

    public bool ShowNews { get; set; } = true;

    /// <summary>Thứ tự section: bannerPromo, midPromo, productTabs, newArrivals, news</summary>
    public List<string> SectionOrder { get; set; } =
    [
        "bannerPromo",
        "midPromo",
        "productTabs",
        "newArrivals",
        "news"
    ];

    /// <summary>best | hot | combo</summary>
    public string DefaultProductTab { get; set; } = "best";

    /// <summary>Tùy chỉnh tiêu đề khu vực hàng mới — null/empty dùng i18n FE.</summary>
    public string? NewArrivalsTitle { get; set; }

    /// <summary>Số bài tin trên trang chủ (1–30).</summary>
    public int HomeNewsTake { get; set; } = 10;

    /// <summary>Ô promo giữa trang — rỗng thì FE dùng mặc định + i18n.</summary>
    public List<HomeMidPromoCardDto> MidPromoCards { get; set; } = [];
}

public class HomeMidPromoCardDto
{
    public string? Title { get; set; }

    public string ImageUrl { get; set; } = "";

    public string? Link { get; set; }
}
