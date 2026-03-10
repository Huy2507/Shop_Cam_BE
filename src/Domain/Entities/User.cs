using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Shop_Cam_BE.Domain.Entities;

[Table("user")]
[Index("KeycloakId", Name = "user_keycloak_id_key", IsUnique = true)]
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

    [Column("keycloak_id")]
    public Guid? KeycloakId { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
