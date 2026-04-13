using System.Text.Json;
using Shop_Cam_BE.Application.DTOs;
using Shop_Cam_BE.Domain.Entities;

namespace Shop_Cam_BE.Application.Common;

/// <summary>Gộp SiteSettings nhóm HomePage thành HomePagePublicDto.</summary>
public static class HomePageConfigMerge
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    private static readonly string[] AllSections =
    [
        "bannerPromo",
        "midPromo",
        "productTabs",
        "newArrivals",
        "news",
    ];

    public static HomePagePublicDto BuildFromSiteRows(IEnumerable<SiteSetting> rows)
    {
        var dto = new HomePagePublicDto();
        foreach (var row in rows)
            ApplySiteKey(dto, row.Key, row.ValueJson);

        Normalize(dto);
        return dto;
    }

    private static void ApplySiteKey(HomePagePublicDto dto, string key, string valueJson)
    {
        try
        {
            switch (key.ToLowerInvariant())
            {
                case "home_show_banner_block":
                    if (TryReadBool(valueJson, out var b1))
                        dto.ShowBannerBlock = b1;
                    break;
                case "home_show_promo_sidebar":
                    if (TryReadBool(valueJson, out var b2))
                        dto.ShowPromoSidebar = b2;
                    break;
                case "home_show_mid_promo":
                    if (TryReadBool(valueJson, out var b3))
                        dto.ShowMidPromo = b3;
                    break;
                case "home_show_product_tabs":
                    if (TryReadBool(valueJson, out var b4))
                        dto.ShowProductTabs = b4;
                    break;
                case "home_show_new_arrivals":
                    if (TryReadBool(valueJson, out var b5))
                        dto.ShowNewArrivals = b5;
                    break;
                case "home_show_news":
                    if (TryReadBool(valueJson, out var b6))
                        dto.ShowNews = b6;
                    break;
                case "home_section_order":
                    ParseSectionOrder(dto, valueJson);
                    break;
                case "home_default_product_tab":
                    var tab = UnquoteString(valueJson);
                    if (tab != null && IsValidProductTab(tab))
                        dto.DefaultProductTab = tab.ToLowerInvariant();
                    break;
                case "home_new_arrivals_title":
                    dto.NewArrivalsTitle = UnquoteString(valueJson);
                    break;
                case "home_news_take":
                    if (TryReadInt(valueJson, out var take))
                        dto.HomeNewsTake = take;
                    break;
                case "home_mid_promo_json":
                    ParseMidPromo(dto, valueJson);
                    break;
            }
        }
        catch
        {
            // bỏ qua giá trị lỗi
        }
    }

    private static void ParseSectionOrder(HomePagePublicDto dto, string valueJson)
    {
        try
        {
            var trimmed = valueJson.Trim();
            List<string>? list = null;
            if (trimmed.StartsWith('['))
                list = JsonSerializer.Deserialize<List<string>>(trimmed, JsonOpts);
            else
            {
                var s = UnquoteString(trimmed);
                if (!string.IsNullOrEmpty(s))
                    list = JsonSerializer.Deserialize<List<string>>(s, JsonOpts);
            }

            if (list == null || list.Count == 0)
                return;

            var ordered = new List<string>();
            foreach (var id in list)
            {
                var canon = TryCanonSection(id);
                if (canon == null)
                    continue;
                if (ordered.Any(x => x.Equals(canon, StringComparison.OrdinalIgnoreCase)))
                    continue;
                ordered.Add(canon);
            }

            foreach (var def in AllSections)
            {
                if (!ordered.Any(x => x.Equals(def, StringComparison.OrdinalIgnoreCase)))
                    ordered.Add(def);
            }

            dto.SectionOrder = ordered;
        }
        catch
        {
            // giữ mặc định ctor
        }
    }

    private static void ParseMidPromo(HomePagePublicDto dto, string valueJson)
    {
        try
        {
            var trimmed = valueJson.Trim();
            if (string.IsNullOrEmpty(trimmed) || trimmed == "[]" || trimmed == "null")
            {
                dto.MidPromoCards = [];
                return;
            }

            var list = JsonSerializer.Deserialize<List<HomeMidPromoCardDto>>(trimmed, JsonOpts);
            if (list == null)
            {
                dto.MidPromoCards = [];
                return;
            }

            dto.MidPromoCards = list
                .Where(x => x != null && !string.IsNullOrWhiteSpace(x.ImageUrl))
                .Take(6)
                .ToList();
        }
        catch
        {
            dto.MidPromoCards = [];
        }
    }

    private static void Normalize(HomePagePublicDto dto)
    {
        dto.HomeNewsTake = Math.Clamp(dto.HomeNewsTake, 1, 30);
        dto.DefaultProductTab = (dto.DefaultProductTab ?? "best").ToLowerInvariant();
        if (!IsValidProductTab(dto.DefaultProductTab))
            dto.DefaultProductTab = "best";

        if (dto.SectionOrder == null || dto.SectionOrder.Count == 0)
            dto.SectionOrder = [.. AllSections];
        else
        {
            var next = new List<string>();
            foreach (var id in dto.SectionOrder)
            {
                var canon = TryCanonSection(id);
                if (canon == null)
                    continue;
                if (next.Any(x => x.Equals(canon, StringComparison.OrdinalIgnoreCase)))
                    continue;
                next.Add(canon);
            }

            foreach (var def in AllSections)
            {
                if (!next.Any(x => x.Equals(def, StringComparison.OrdinalIgnoreCase)))
                    next.Add(def);
            }

            dto.SectionOrder = next;
        }
    }

    private static string? TryCanonSection(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;
        return AllSections.FirstOrDefault(s => s.Equals(raw.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsValidProductTab(string v) =>
        v.Equals("best", StringComparison.OrdinalIgnoreCase)
        || v.Equals("hot", StringComparison.OrdinalIgnoreCase)
        || v.Equals("combo", StringComparison.OrdinalIgnoreCase);

    private static string? UnquoteString(string valueJson)
    {
        var trimmed = valueJson.Trim();
        if (trimmed.StartsWith('"') && trimmed.EndsWith('"') && trimmed.Length >= 2)
        {
            try
            {
                return JsonSerializer.Deserialize<string>(trimmed);
            }
            catch
            {
                return trimmed.Trim('"');
            }
        }

        try
        {
            return JsonSerializer.Deserialize<string>(trimmed);
        }
        catch
        {
            return trimmed;
        }
    }

    private static bool TryReadInt(string valueJson, out int v)
    {
        v = 0;
        var maybe = UnquoteString(valueJson);
        if (!string.IsNullOrEmpty(maybe) && int.TryParse(maybe, out v))
            return true;
        var t = valueJson.Trim();
        if (int.TryParse(t, out v))
            return true;
        try
        {
            v = JsonSerializer.Deserialize<int>(t);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool TryReadBool(string valueJson, out bool v)
    {
        v = false;
        var maybe = UnquoteString(valueJson);
        if (!string.IsNullOrEmpty(maybe) && bool.TryParse(maybe, out v))
            return true;
        var t = valueJson.Trim();
        if (bool.TryParse(t, out v))
            return true;
        try
        {
            v = JsonSerializer.Deserialize<bool>(t);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
