namespace Shop_Cam_BE.Domain.Entities;

/// <summary>Bảng nối user ↔ role (nhiều-nhiều).</summary>
public class UserRole
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }

    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}
