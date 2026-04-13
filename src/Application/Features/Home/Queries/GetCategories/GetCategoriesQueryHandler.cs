using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Domain.Entities;

namespace Shop_Cam_BE.Application.Features.Home.Queries.GetCategories;

/// <summary>
/// Đọc ProductCategories (AsNoTracking), sắp theo tên.
/// </summary>
public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<ProductCategory>>
{
    private readonly IApplicationDbContext _context;

    public GetCategoriesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductCategory>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await _context.ProductCategories
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }
}

