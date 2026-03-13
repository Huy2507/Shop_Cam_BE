namespace Shop_Cam_BE.Domain.Entities;

/// <summary>
/// Dòng chi tiết sản phẩm trong một đơn hàng.
/// </summary>
public class OrderItem
{
    public Guid OrderItemId { get; set; }

    public Guid OrderId { get; set; }

    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = default!;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal LineTotal { get; set; }

    public Order? Order { get; set; }
}

