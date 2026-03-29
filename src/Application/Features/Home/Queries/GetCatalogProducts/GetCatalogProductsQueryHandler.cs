using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Extensions;
using Shop_Cam_BE.Application.Common.Helpers;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.DTOs;
using Shop_Cam_BE.Domain.Entities;

namespace Shop_Cam_BE.Application.Features.Home.Queries.GetCatalogProducts;

/// <summary>Danh sách sản phẩm catalog (lọc, sort, phân trang).</summary>
public class GetCatalogProductsQueryHandler : IRequestHandler<GetCatalogProductsQuery, CatalogProductsResult>
{
    private readonly IApplicationDbContext _context;

    public GetCatalogProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CatalogProductsResult> Handle(GetCatalogProductsQuery request, CancellationToken cancellationToken)
    {
        var page = StorefrontPagination.NormalizePage(request.Page);
        var pageSize = StorefrontPagination.NormalizeCatalogPageSize(request.PageSize);

        IQueryable<Product> query = _context.Products.AsNoTracking();

        query = query
            .ApplyStorefrontHomeTabOrFilter(request.HomeTab)
            .ApplyStorefrontCategoryName(request.CategoryName)
            .ApplyStorefrontNameSearch(request.Search)
            .ApplyStorefrontMinEffectivePrice(request.MinPrice)
            .ApplyStorefrontMaxEffectivePrice(request.MaxPrice)
            .ApplyStorefrontCatalogSort(request.Sort);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .SelectAsCatalogProductDto()
            .ToListAsync(cancellationToken);

        return new CatalogProductsResult
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }
}
