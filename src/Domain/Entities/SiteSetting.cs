using Shop_Cam_BE.Domain.Common;

namespace Shop_Cam_BE.Domain.Entities;

/// <summary>
/// Cấu hình key–value theo nhóm (UI, SEO, …) cho phép mở rộng không cần migration mỗi tham số.
/// </summary>
public class SiteSetting : IAuditableSoftDeletable
{
    public Guid SiteSettingId { get; set; }

    /// <summary>Nhóm: UI, Display, SEO, Ads, …</summary>
    public string Group { get; set; } = default!;

    public string Key { get; set; } = default!;

    /// <summary>Giá trị JSON (chuỗi, số, object tùy key).</summary>
    public string ValueJson { get; set; } = "{}";

    public string? Description { get; set; }

    public User? UpdatedByUser { get; set; }

    public DateTimeOffset CreatedTime { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public DateTimeOffset UpdatedTime { get; set; }
    public Guid? UpdatedByUserId { get; set; }
    public bool IsActive { get; set; } = true;
}
