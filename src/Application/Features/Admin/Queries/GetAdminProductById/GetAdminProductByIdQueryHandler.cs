using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Admin.Queries.GetAdminProductById;

/// <summary>
/// Project từ Products sang DTO chi tiết (AsNoTracking).
/// </summary>
public class GetAdminProductByIdQueryHandler : IRequestHandler<GetAdminProductByIdQuery, AdminProductDetailDto?>
{
    private readonly IApplicationDbContext _context;

    public GetAdminProductByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AdminProductDetailDto?> Handle(GetAdminProductByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(p => p.ProductId == request.ProductId)
            .Select(p => new AdminProductDetailDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Price = p.Price,
                Discount = p.Discount,
                Info = p.Info,
                Description = p.Description,
                ImageUrl = p.ImageUrl,
                IsNew = p.IsNew,
                OutOfStock = p.OutOfStock,
                Badge = p.Badge,
                ProductCategoryId = p.ProductCategoryId,
                CategoryName = p.Category != null ? p.Category.Name : null,
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
