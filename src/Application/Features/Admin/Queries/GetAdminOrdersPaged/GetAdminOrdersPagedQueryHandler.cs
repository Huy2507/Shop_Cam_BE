using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Helpers;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Admin.Queries.GetAdminOrdersPaged;

/// <summary>
/// Join Orders với user nếu cần hiển thị; lọc và phân trang theo tham số query.
/// </summary>
public class GetAdminOrdersPagedQueryHandler : IRequestHandler<GetAdminOrdersPagedQuery, PagedResult<AdminOrderListItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAdminOrdersPagedQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<AdminOrderListItemDto>> Handle(
        GetAdminOrdersPagedQuery request,
        CancellationToken cancellationToken)
    {
        var page = AdminPagination.NormalizePage(request.Page);
        var pageSize = AdminPagination.NormalizePageSize(request.PageSize);
        var search = request.Search?.Trim();

        var q = _context.Orders.AsNoTracking();
        if (!string.IsNullOrEmpty(search))
        {
            q = q.Where(o =>
                o.Code.Contains(search) ||
                o.Phone.Contains(search) ||
                o.CustomerName.Contains(search) ||
                (o.Email != null && o.Email.Contains(search)));
        }

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderByDescending(o => o.CreatedTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new AdminOrderListItemDto
            {
                OrderId = o.OrderId,
                Code = o.Code,
                CustomerName = o.CustomerName,
                Phone = o.Phone,
                Email = o.Email,
                TotalAmount = o.TotalAmount,
                CreatedAt = o.CreatedTime.UtcDateTime,
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<AdminOrderListItemDto>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize,
        };
    }
}
