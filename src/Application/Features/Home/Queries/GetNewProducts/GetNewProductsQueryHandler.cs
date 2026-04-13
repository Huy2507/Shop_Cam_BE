using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Extensions;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Domain.Entities;

namespace Shop_Cam_BE.Application.Features.Home.Queries.GetNewProducts;

/// <summary>
/// Xử lý truy vấn lấy danh sách sản phẩm mới cho khu vực "Bộ sưu tập mới".
/// </summary>
public class GetNewProductsQueryHandler : IRequestHandler<GetNewProductsQuery, List<Product>>
{
    private readonly IApplicationDbContext _context;

    public GetNewProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> Handle(GetNewProductsQuery request, CancellationToken cancellationToken)
    {
        // Lọc theo flag IsNew, giới hạn số lượng để hiển thị gọn gàng.
        return await _context.Products
            .WhereStorefrontActive()
            .Where(p => p.IsNew)
            .Take(20)
            .ToListAsync(cancellationToken);
    }
}

