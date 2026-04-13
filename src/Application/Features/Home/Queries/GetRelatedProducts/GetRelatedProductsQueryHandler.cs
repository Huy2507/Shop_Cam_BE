using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Extensions;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Home.Queries.GetRelatedProducts;

/// <summary>Sản phẩm liên quan (cùng danh mục).</summary>
public class GetRelatedProductsQueryHandler : IRequestHandler<GetRelatedProductsQuery, List<CatalogProductDto>>
{
    private readonly IApplicationDbContext _context;

    public GetRelatedProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CatalogProductDto>> Handle(GetRelatedProductsQuery request, CancellationToken cancellationToken)
    {
        var take = Math.Clamp(request.Take, 1, 12);
        var current = await _context.Products
            .AsNoTracking()
            .WhereStorefrontActive()
            .FirstOrDefaultAsync(p => p.ProductId == request.ProductId, cancellationToken);

        if (current?.ProductCategoryId == null)
            return new List<CatalogProductDto>();

        var catId = current.ProductCategoryId.Value;

        return await _context.Products
            .AsNoTracking()
            .WhereStorefrontActive()
            .Where(p => p.ProductCategoryId == catId && p.ProductId != request.ProductId)
            .OrderByDescending(p => p.IsNew)
            .ThenByDescending(p => p.ProductId)
            .Take(take)
            .SelectAsCatalogProductDto()
            .ToListAsync(cancellationToken);
    }
}
