using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common;
using Shop_Cam_BE.Application.Common.Constants;
using Shop_Cam_BE.Application.Common.Helpers;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.SiteSettings.Queries.GetPublicUiSettings;

/// <summary>
/// Gộp SiteSettings thành PublicUiConfigDto; parse JSON từng key trong ApplyKey; trang tin gộp tùy user.
/// </summary>
public class GetPublicUiSettingsQueryHandler : IRequestHandler<GetPublicUiSettingsQuery, PublicUiConfigDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetPublicUiSettingsQueryHandler(
        IApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PublicUiConfigDto> Handle(GetPublicUiSettingsQuery request, CancellationToken cancellationToken)
    {
        var dto = DefaultUi();
        var rows = await _context.SiteSettings
            .AsNoTracking()
            .Where(x => x.IsActive && (
                x.Group == SiteSettingGroups.Ui
                || x.Group == SiteSettingGroups.Integrations
                || x.Group == SiteSettingGroups.NewsPage
                || x.Group == SiteSettingGroups.HomePage))
            .ToListAsync(cancellationToken);

        foreach (var row in rows)
        {
            if (row.Group == SiteSettingGroups.NewsPage || row.Group == SiteSettingGroups.HomePage)
                continue;
            ApplyKey(dto, row.Key, row.ValueJson);
        }

        dto.HomePage = HomePageConfigMerge.BuildFromSiteRows(rows.Where(r => r.Group == SiteSettingGroups.HomePage));

        var newsSite = NewsPageConfigMerge.BuildFromSiteRows(rows.Where(r => r.Group == SiteSettingGroups.NewsPage));
        var userId = TryGetUserIdFromCookie();
        if (userId != null)
        {
            var pref = await _context.UserNewsPagePreferences.AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
            if (pref != null)
                NewsPageConfigMerge.ApplyUserJson(newsSite, pref.ValueJson, out _);
        }

        dto.NewsPage = newsSite;
        return dto;
    }

    private Guid? TryGetUserIdFromCookie()
    {
        var token = _httpContextAccessor.HttpContext?.Request.Cookies["access_token"];
        if (string.IsNullOrEmpty(token))
            return null;
        var sub = JwtHelper.ExtractSubFromJwt(token);
        return Guid.TryParse(sub, out var id) ? id : null;
    }

    private static PublicUiConfigDto DefaultUi() => new();

    private static void ApplyKey(PublicUiConfigDto dto, string key, string valueJson)
    {
        try
        {
            switch (key.ToLowerInvariant())
            {
                case "theme_primary_color":
                    dto.ThemePrimaryColor = UnquoteString(valueJson) ?? dto.ThemePrimaryColor;
                    break;
                case "theme_accent_color":
                    dto.ThemeAccentColor = UnquoteString(valueJson) ?? dto.ThemeAccentColor;
                    break;
                case "logo_url":
                    dto.LogoUrl = UnquoteString(valueJson);
                    break;
                case "favicon_url":
                    dto.FaviconUrl = UnquoteString(valueJson);
                    break;
                case "header_background":
                    dto.HeaderBackground = UnquoteString(valueJson) ?? dto.HeaderBackground;
                    break;
                case "hero_tagline":
                    dto.HeroTagline = UnquoteString(valueJson);
                    break;
                case "zalo_oa_id":
                    dto.ZaloOaId = UnquoteString(valueJson);
                    break;
                case "zalo_chat_url":
                    dto.ZaloChatUrl = UnquoteString(valueJson);
                    break;
                case "zalo_welcome_message":
                    dto.ZaloWelcomeMessage = UnquoteString(valueJson);
                    break;
            }
        }
        catch
        {
            // bỏ qua giá trị JSON lỗi
        }
    }

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
}
