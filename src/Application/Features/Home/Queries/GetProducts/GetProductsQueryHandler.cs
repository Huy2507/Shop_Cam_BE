using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Extensions;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Domain.Entities;

namespace Shop_Cam_BE.Application.Features.Home.Queries.GetProducts;

/// <summary>Danh sách sản phẩm trang chủ (filter best/hot/combo).</summary>
public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<Product>>
{
    private readonly IApplicationDbContext _context;

    public GetProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Product> query = _context.Products;
        query = query.ApplyStorefrontHomeTabOrFilter(request.Filter);
        return await query
            .Take(20)
            .ToListAsync(cancellationToken);
    }
}
