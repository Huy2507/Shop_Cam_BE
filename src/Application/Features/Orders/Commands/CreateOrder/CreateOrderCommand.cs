using MediatR;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Orders.Commands.CreateOrder;

/// <summary>
/// Command tạo đơn hàng mới từ FE.
/// </summary>
public class CreateOrderCommand : IRequest<Result<CreateOrderResultDto>>
{
    public required CreateOrderDto Order { get; set; }
}

