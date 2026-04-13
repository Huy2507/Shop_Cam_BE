using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop_Cam_BE.Domain.Entities;

/// <summary>Tùy chọn hiển thị trang tin (layout, page size…) theo từng user đăng nhập.</summary>
[Table("UserNewsPagePreferences")]
public class UserNewsPagePreference
{
    [Key]
    public Guid UserNewsPagePreferenceId { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    /// <summary>JSON: layout, pageSize, showFeatured, gridColumns (từng field optional).</summary>
    public string ValueJson { get; set; } = "{}";

    public DateTimeOffset UpdatedAt { get; set; }
}
