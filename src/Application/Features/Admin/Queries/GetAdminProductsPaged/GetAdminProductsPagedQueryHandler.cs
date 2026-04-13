using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Helpers;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Admin.Queries.GetAdminProductsPaged;

/// <summary>
/// Áp dụng tìm kiếm Contains, lọc CategoryId, sắp theo tên rồi Skip/Take.
/// </summary>
public class GetAdminProductsPagedQueryHandler : IRequestHandler<GetAdminProductsPagedQuery, PagedResult<AdminProductListItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAdminProductsPagedQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<AdminProductListItemDto>> Handle(
        GetAdminProductsPagedQuery request,
        CancellationToken cancellationToken)
    {
        var page = AdminPagination.NormalizePage(request.Page);
        var pageSize = AdminPagination.NormalizePageSize(request.PageSize);

        var q = _context.Products.AsNoTracking();
        var search = request.Search?.Trim();
        if (!string.IsNullOrEmpty(search))
            q = q.Where(p => p.Name.Contains(search));
        if (request.CategoryId.HasValue)
            q = q.Where(p => p.ProductCategoryId == request.CategoryId);

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new AdminProductListItemDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Price = p.Price,
                Discount = p.Discount,
                ImageUrl = p.ImageUrl,
                IsNew = p.IsNew,
                OutOfStock = p.OutOfStock,
                ProductCategoryId = p.ProductCategoryId,
                CategoryName = p.Category != null ? p.Category.Name : null,
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<AdminProductListItemDto>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize,
        };
    }
}
