namespace Shop_Cam_BE.Domain.Entities;

/// <summary>
/// Đơn hàng đơn giản lưu lại thông tin khách đặt hàng từ FE.
/// </summary>
public class Order
{
    public Guid OrderId { get; set; }

    public string Code { get; set; } = default!;

    public string CustomerName { get; set; } = default!;

    public string Phone { get; set; } = default!;

    public string? Email { get; set; }

    public string Address { get; set; } = default!;

    public string? Note { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}

