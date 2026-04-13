using MediatR;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.SiteSettings.Queries.GetSiteSettingsByGroup;

/// <summary>
/// Tất cả key trong một nhóm cấu hình (vd: UI) cho màn admin.
/// </summary>
public class GetSiteSettingsByGroupQuery : IRequest<List<SiteSettingItemDto>>
{
    public required string Group { get; set; }
}
