namespace Shop_Cam_BE.Domain.Common;

/// <summary>
/// Dấu thời gian và người thao tác; <see cref="IsActive"/> = false là xóa mềm.
/// </summary>
public interface IAuditableSoftDeletable
{
    DateTimeOffset CreatedTime { get; set; }
    Guid? CreatedByUserId { get; set; }
    DateTimeOffset UpdatedTime { get; set; }
    Guid? UpdatedByUserId { get; set; }
    bool IsActive { get; set; }
}
