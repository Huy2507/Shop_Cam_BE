using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Admin.Queries.GetAdminOrderById;

/// <summary>
/// Include OrderItems và map sang DTO chi tiết.
/// </summary>
public class GetAdminOrderByIdQueryHandler : IRequestHandler<GetAdminOrderByIdQuery, AdminOrderDetailDto?>
{
    private readonly IApplicationDbContext _context;

    public GetAdminOrderByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AdminOrderDetailDto?> Handle(GetAdminOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.OrderId == request.OrderId, cancellationToken);
        if (order == null)
            return null;

        return new AdminOrderDetailDto
        {
            OrderId = order.OrderId,
            Code = order.Code,
            CustomerName = order.CustomerName,
            Phone = order.Phone,
            Email = order.Email,
            Address = order.Address,
            Note = order.Note,
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedTime.UtcDateTime,
            Items = order.Items
                .OrderBy(i => i.OrderItemId)
                .Select(i => new AdminOrderLineDto
                {
                    OrderItemId = i.OrderItemId,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    LineTotal = i.LineTotal,
                })
                .ToList(),
        };
    }
}
