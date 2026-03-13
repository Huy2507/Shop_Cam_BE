using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop_Cam_BE.Application.DTOs;
using Shop_Cam_BE.Application.Features.Orders.Commands.CreateOrder;

namespace Shop_Cam_BE.Web.Controllers;

/// <summary>
/// API xử lý đơn hàng được tạo từ FE (không yêu cầu đăng nhập).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Tạo đơn hàng mới từ thông tin giỏ hàng + thông tin khách.
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto, CancellationToken cancellationToken)
    {
        var command = new CreateOrderCommand { Order = dto };
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.Succeeded)
        {
            return BadRequest(result);
        }

        return Ok(result.Value);
    }
}

