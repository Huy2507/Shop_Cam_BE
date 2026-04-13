using MediatR;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.SiteSettings.Commands.UpsertSiteSettings;

/// <summary>
/// Ghi đè hoặc tạo nhiều bản ghi cấu hình site (whitelist Group/Key theo SiteSettingGroups).
/// </summary>
public class UpsertSiteSettingsCommand : IRequest<Result<Unit>>
{
    public List<UpsertSiteSettingItemDto> Items { get; set; } = new();
}
