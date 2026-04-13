using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Admin.Queries.GetAdminCategoryById;

/// <summary>
/// Map ProductCategories sang DTO chi tiết.
/// </summary>
public class GetAdminCategoryByIdQueryHandler : IRequestHandler<GetAdminCategoryByIdQuery, AdminCategoryDetailDto?>
{
    private readonly IApplicationDbContext _context;

    public GetAdminCategoryByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AdminCategoryDetailDto?> Handle(GetAdminCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.ProductCategories
            .AsNoTracking()
            .Where(c => c.ProductCategoryId == request.CategoryId)
            .Select(c => new AdminCategoryDetailDto
            {
                ProductCategoryId = c.ProductCategoryId,
                Name = c.Name,
                Slug = c.Slug,
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
