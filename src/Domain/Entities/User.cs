using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop_Cam_BE.Domain.Entities;

[Table("user")]
public partial class User
{
    [Key]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("user_name")]
    [StringLength(255)]
    public string UserName { get; set; } = null!;

    [Column("first_name")]
    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [Column("last_name")]
    [StringLength(100)]
    public string LastName { get; set; } = null!;

    [Column("email")]
    [StringLength(255)]
    public string Email { get; set; } = null!;

    [Column("phone")]
    [StringLength(20)]
    public string? Phone { get; set; }

    /// <summary>Hash mật khẩu (ASP.NET Identity PasswordHasher). Null = chưa đặt mật khẩu cục bộ.</summary>
    [Column("password_hash")]
    public string? PasswordHash { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    /// <summary>Người tạo bản ghi (nullable nếu import/seed).</summary>
    [Column("created_by_user_id")]
    public Guid? CreatedByUserId { get; set; }

    /// <summary>Người cập nhật gần nhất.</summary>
    [Column("updated_by_user_id")]
    public Guid? UpdatedByUserId { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
