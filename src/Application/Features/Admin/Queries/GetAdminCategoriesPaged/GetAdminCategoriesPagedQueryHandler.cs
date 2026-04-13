using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Helpers;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Admin.Queries.GetAdminCategoriesPaged;

/// <summary>
/// Lọc ProductCategories theo tên, đếm tổng và phân trang.
/// </summary>
public class GetAdminCategoriesPagedQueryHandler : IRequestHandler<GetAdminCategoriesPagedQuery, PagedResult<AdminCategoryListItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAdminCategoriesPagedQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<AdminCategoryListItemDto>> Handle(
        GetAdminCategoriesPagedQuery request,
        CancellationToken cancellationToken)
    {
        var page = AdminPagination.NormalizePage(request.Page);
        var pageSize = AdminPagination.NormalizePageSize(request.PageSize);
        var search = request.Search?.Trim();

        var q = _context.ProductCategories.AsNoTracking();
        if (!string.IsNullOrEmpty(search))
            q = q.Where(c => c.Name.Contains(search));

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new AdminCategoryListItemDto
            {
                ProductCategoryId = c.ProductCategoryId,
                Name = c.Name,
                Slug = c.Slug,
                ProductCount = c.Products.Count,
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<AdminCategoryListItemDto>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize,
        };
    }
}
