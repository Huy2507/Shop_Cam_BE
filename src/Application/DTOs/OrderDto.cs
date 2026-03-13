using System.ComponentModel.DataAnnotations;

namespace Shop_Cam_BE.Application.DTOs;

/// <summary>
/// Item trong request tạo đơn hàng gửi từ FE.
/// </summary>
public class CreateOrderItemDto
{
    [Required]
    public Guid ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}

/// <summary>
/// Request tạo đơn hàng từ màn giỏ hàng.
/// </summary>
public class CreateOrderDto
{
    [Required]
    [MaxLength(200)]
    public string CustomerName { get; set; } = default!;

    [Required]
    [MaxLength(50)]
    public string Phone { get; set; } = default!;

    [EmailAddress]
    [MaxLength(200)]
    public string? Email { get; set; }

    [Required]
    [MaxLength(500)]
    public string Address { get; set; } = default!;

    [MaxLength(1000)]
    public string? Note { get; set; }

    [MinLength(1)]
    public List<CreateOrderItemDto> Items { get; set; } = new();
}

/// <summary>
/// Thông tin trả về cho FE sau khi tạo đơn thành công.
/// </summary>
public class CreateOrderResultDto
{
    public Guid OrderId { get; set; }
    public string Code { get; set; } = default!;
    public decimal TotalAmount { get; set; }
}

