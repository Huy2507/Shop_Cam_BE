using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shop_Cam_BE.Application.Common.Constants;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;
using Shop_Cam_BE.Domain.Entities;

namespace Shop_Cam_BE.Application.Features.Orders.Commands.CreateOrder;

/// <summary>
/// Xử lý luồng tạo đơn hàng từ màn giỏ hàng.
/// </summary>
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<CreateOrderResultDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(IApplicationDbContext context, ILogger<CreateOrderCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<CreateOrderResultDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Order;

        if (dto.Items == null || dto.Items.Count == 0)
        {
            return Result<CreateOrderResultDto>.Failure(ErrorCodes.ORDER_REQUIRES_ITEMS);
        }

        // Lấy danh sách productId duy nhất từ request.
        var productIds = dto.Items
            .Select(i => i.ProductId)
            .Distinct()
            .ToList();

        var products = await _context.Products
            .Where(p => productIds.Contains(p.ProductId) && p.IsActive)
            .ToListAsync(cancellationToken);

        if (products.Count != productIds.Count)
        {
            return Result<CreateOrderResultDto>.Failure(ErrorCodes.ORDER_PRODUCTS_NOT_FOUND);
        }

        // Tính toán chi tiết đơn hàng.
        var orderItems = new List<OrderItem>();
        decimal totalAmount = 0m;

        foreach (var item in dto.Items)
        {
            var product = products.First(p => p.ProductId == item.ProductId);

            var unitPrice = product.Discount.HasValue && product.Discount.Value > 0
                ? product.Price - product.Discount.Value
                : product.Price;

            var lineTotal = unitPrice * item.Quantity;
            totalAmount += lineTotal;

            orderItems.Add(new OrderItem
            {
                OrderItemId = Guid.NewGuid(),
                ProductId = product.ProductId,
                ProductName = product.Name,
                UnitPrice = unitPrice,
                Quantity = item.Quantity,
                LineTotal = lineTotal
            });
        }

        var order = new Order
        {
            OrderId = Guid.NewGuid(),
            Code = GenerateOrderCode(),
            CustomerName = dto.CustomerName.Trim(),
            Phone = dto.Phone.Trim(),
            Email = dto.Email?.Trim(),
            Address = dto.Address.Trim(),
            Note = dto.Note?.Trim(),
            TotalAmount = totalAmount,
            Items = orderItems
        };

        await _context.Orders.AddAsync(order, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Tạo đơn hàng mới {OrderCode} với tổng tiền {TotalAmount}", order.Code, order.TotalAmount);

        var resultDto = new CreateOrderResultDto
        {
            OrderId = order.OrderId,
            Code = order.Code,
            TotalAmount = order.TotalAmount
        };

        return Result<CreateOrderResultDto>.Success(resultDto);
    }

    /// <summary>
    /// Sinh mã đơn hàng dạng đơn giản: DHyyyyMMdd-HHmmss-xxxx.
    /// </summary>
    private static string GenerateOrderCode()
    {
        var now = DateTime.UtcNow;
        var randomPart = Random.Shared.Next(1000, 9999);
        return $"DH{now:yyyyMMdd-HHmmss}-{randomPart}";
    }
}

