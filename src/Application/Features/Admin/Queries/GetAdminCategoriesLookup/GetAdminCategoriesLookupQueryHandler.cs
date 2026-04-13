using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Admin.Queries.GetAdminCategoriesLookup;

/// <summary>
/// Sắp theo tên, chỉ các trường cần cho select box.
/// </summary>
public class GetAdminCategoriesLookupQueryHandler : IRequestHandler<GetAdminCategoriesLookupQuery, List<AdminCategoryLookupDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAdminCategoriesLookupQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AdminCategoryLookupDto>> Handle(
        GetAdminCategoriesLookupQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.ProductCategories
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .Select(c => new AdminCategoryLookupDto
            {
                ProductCategoryId = c.ProductCategoryId,
                Name = c.Name,
            })
            .ToListAsync(cancellationToken);
    }
}
