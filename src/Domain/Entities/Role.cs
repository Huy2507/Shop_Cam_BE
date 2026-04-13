using Shop_Cam_BE.Domain.Common;

namespace Shop_Cam_BE.Domain.Entities;

/// <summary>Vai trò trong hệ thống (ánh xạ quyền API, ví dụ Admin).</summary>
public class Role : IAuditableSoftDeletable
{
    public Guid RoleId { get; set; }

    public string Name { get; set; } = null!;

    /// <summary>Tên chuẩn hóa để so khớp (vd: ADMIN).</summary>
    public string NormalizedName { get; set; } = null!;

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public DateTimeOffset CreatedTime { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public DateTimeOffset UpdatedTime { get; set; }
    public Guid? UpdatedByUserId { get; set; }
    public bool IsActive { get; set; } = true;
}
