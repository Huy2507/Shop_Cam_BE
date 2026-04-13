using MediatR;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Admin.Queries.GetAdminOrderById;

/// <summary>
/// Chi tiết đơn hàng kèm dòng OrderItems cho màn admin.
/// </summary>
public class GetAdminOrderByIdQuery : IRequest<AdminOrderDetailDto?>
{
    public Guid OrderId { get; set; }
}
