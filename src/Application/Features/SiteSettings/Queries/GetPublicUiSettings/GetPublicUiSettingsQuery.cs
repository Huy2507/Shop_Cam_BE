using MediatR;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.SiteSettings.Queries.GetPublicUiSettings;

/// <summary>
/// Cấu hình UI công khai cho storefront (đọc từ nhóm UI + Integrations).
/// </summary>
public class GetPublicUiSettingsQuery : IRequest<PublicUiConfigDto>
{
}
