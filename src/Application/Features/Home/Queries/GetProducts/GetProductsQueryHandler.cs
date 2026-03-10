using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Domain.Entities;

namespace Shop_Cam_BE.Application.Features.Home.Queries.GetProducts;

/// <summary>
/// Xử lý truy vấn lấy danh sách sản phẩm cho khu vực Product List.
/// </summary>
public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<Product>>
{
    private readonly IApplicationDbContext _context;

    public GetProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        // Bắt đầu từ toàn bộ sản phẩm.
        IQueryable<Product> query = _context.Products;

        // Chuẩn hoá filter về chữ thường để so sánh.
        var filter = request.Filter?.Trim().ToLowerInvariant();

        // "hot": sản phẩm có Discount > 0.
        if (filter == "hot")
        {
            query = query.Where(p => p.Discount != null && p.Discount > 0);
        }
        // "combo": tạm thời dùng logic Skip(2) giống mock ban đầu (có thể đổi sang flag riêng sau).
        else if (filter == "combo")
        {
            query = query.Skip(2);
        }

        // Giới hạn số lượng trả về để tránh query quá nặng cho trang chủ.
        return await query
            .Take(20)
            .ToListAsync(cancellationToken);
    }
}

