using Shop_Cam_BE.Domain.Common;

namespace Shop_Cam_BE.Domain.Entities;

public class HomeBanner : IAuditableSoftDeletable
{
    public Guid HomeBannerId { get; set; }
    public string UrlImg { get; set; } = default!;
    public string? Title { get; set; }
    public string? Link { get; set; }
    public bool IsMain { get; set; }
    public int DisplayOrder { get; set; }

    public DateTimeOffset CreatedTime { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public DateTimeOffset UpdatedTime { get; set; }
    public Guid? UpdatedByUserId { get; set; }
    public bool IsActive { get; set; } = true;
}
