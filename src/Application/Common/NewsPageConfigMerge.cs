using System.Text.Json;
using Shop_Cam_BE.Application.DTOs;
using Shop_Cam_BE.Domain.Entities;

namespace Shop_Cam_BE.Application.Common;

/// <summary>Gộp SiteSettings nhóm NewsPage + JSON tùy chọn user.</summary>
public static class NewsPageConfigMerge
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public static NewsPagePublicDto BuildFromSiteRows(IEnumerable<SiteSetting> newsPageRows)
    {
        var dto = new NewsPagePublicDto();
        foreach (var row in newsPageRows)
        {
            ApplySiteKey(dto, row.Key, row.ValueJson);
        }

        Normalize(dto);
        return dto;
    }

    public static void ApplyUserJson(NewsPagePublicDto target, string? valueJson, out bool personalized)
    {
        personalized = false;
        if (string.IsNullOrWhiteSpace(valueJson) || valueJson.Trim() == "{}")
            return;

        UserPrefPatch? patch;
        try
        {
            patch = JsonSerializer.Deserialize<UserPrefPatch>(valueJson, JsonOpts);
        }
        catch
        {
            return;
        }

        if (patch == null)
            return;

        if (patch.Layout != null && IsValidLayout(patch.Layout))
        {
            target.Layout = patch.Layout.ToLowerInvariant();
            personalized = true;
        }

        if (patch.PageSize is > 0)
        {
            target.PageSize = ClampPageSize(patch.PageSize.Value);
            personalized = true;
        }

        if (patch.ShowFeatured is bool b)
        {
            target.ShowFeatured = b;
            personalized = true;
        }

        if (patch.GridColumns is >= 2 and <= 4)
        {
            target.GridColumns = patch.GridColumns.Value;
            personalized = true;
        }

        if (patch.VisualTemplate != null && IsValidVisualTemplate(patch.VisualTemplate))
        {
            target.VisualTemplate = patch.VisualTemplate.ToLowerInvariant();
            personalized = true;
        }

        target.Personalized = personalized;
    }

    public static string MergePreferenceJson(string? existingJson, UpsertUserNewsPagePreferenceDto dto)
    {
        UserPrefPatch patch;
        try
        {
            patch = string.IsNullOrWhiteSpace(existingJson) || existingJson.Trim() == "{}"
                ? new UserPrefPatch()
                : JsonSerializer.Deserialize<UserPrefPatch>(existingJson!, JsonOpts) ?? new UserPrefPatch();
        }
        catch
        {
            patch = new UserPrefPatch();
        }

        if (dto.Layout != null)
        {
            if (!IsValidLayout(dto.Layout))
                throw new ArgumentException("Invalid layout", nameof(dto));
            patch.Layout = dto.Layout;
        }

        if (dto.PageSize is { } ps)
        {
            if (ps < 4 || ps > 50)
                throw new ArgumentException("PageSize must be 4–50", nameof(dto));
            patch.PageSize = ps;
        }

        if (dto.ShowFeatured is { } sf)
            patch.ShowFeatured = sf;

        if (dto.GridColumns is { } gc)
        {
            if (gc is < 2 or > 4)
                throw new ArgumentException("GridColumns must be 2–4", nameof(dto));
            patch.GridColumns = gc;
        }

        if (dto.VisualTemplate != null)
        {
            if (!IsValidVisualTemplate(dto.VisualTemplate))
                throw new ArgumentException("Invalid visual template", nameof(dto));
            patch.VisualTemplate = dto.VisualTemplate;
        }

        return JsonSerializer.Serialize(patch, JsonOpts);
    }

    private static void ApplySiteKey(NewsPagePublicDto dto, string key, string valueJson)
    {
        try
        {
            switch (key.ToLowerInvariant())
            {
                case "news_page_title":
                    var t = UnquoteString(valueJson);
                    if (!string.IsNullOrWhiteSpace(t))
                        dto.PageTitle = t!;
                    break;
                case "news_page_subtitle":
                    dto.PageSubtitle = UnquoteString(valueJson);
                    break;
                case "news_default_layout":
                    var l = UnquoteString(valueJson);
                    if (l != null && IsValidLayout(l))
                        dto.Layout = l;
                    break;
                case "news_page_size":
                    if (TryReadInt(valueJson, out var ps))
                        dto.PageSize = ClampPageSize(ps);
                    break;
                case "news_show_featured":
                    if (TryReadBool(valueJson, out var sf))
                        dto.ShowFeatured = sf;
                    break;
                case "news_grid_columns":
                    if (TryReadInt(valueJson, out var gc) && gc is >= 2 and <= 4)
                        dto.GridColumns = gc;
                    break;
                case "news_visual_template":
                    var vt = UnquoteString(valueJson);
                    if (vt != null && IsValidVisualTemplate(vt))
                        dto.VisualTemplate = vt.ToLowerInvariant();
                    break;
            }
        }
        catch
        {
            // bỏ qua giá trị lỗi
        }
    }

    private static void Normalize(NewsPagePublicDto dto)
    {
        dto.PageSize = ClampPageSize(dto.PageSize);
        dto.Layout = (dto.Layout ?? "magazine").ToLowerInvariant();
        if (!IsValidLayout(dto.Layout))
            dto.Layout = "magazine";
        dto.GridColumns = dto.GridColumns is >= 2 and <= 4 ? dto.GridColumns : 4;
        dto.VisualTemplate = (dto.VisualTemplate ?? "classic").ToLowerInvariant();
        if (!IsValidVisualTemplate(dto.VisualTemplate))
            dto.VisualTemplate = "classic";
    }

    private static int ClampPageSize(int n) => Math.Clamp(n, 4, 50);

    private static bool IsValidLayout(string? v) =>
        v != null && (v.Equals("magazine", StringComparison.OrdinalIgnoreCase)
                      || v.Equals("grid", StringComparison.OrdinalIgnoreCase)
                      || v.Equals("list", StringComparison.OrdinalIgnoreCase));

    private static bool IsValidVisualTemplate(string? v) =>
        v != null && (v.Equals("classic", StringComparison.OrdinalIgnoreCase)
                      || v.Equals("editorial", StringComparison.OrdinalIgnoreCase)
                      || v.Equals("minimal", StringComparison.OrdinalIgnoreCase)
                      || v.Equals("bold", StringComparison.OrdinalIgnoreCase)
                      || v.Equals("glass", StringComparison.OrdinalIgnoreCase));

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

    private sealed class UserPrefPatch
    {
        public string? Layout { get; set; }

        public int? PageSize { get; set; }

        public bool? ShowFeatured { get; set; }

        public int? GridColumns { get; set; }

        public string? VisualTemplate { get; set; }
    }
}
