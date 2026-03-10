using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Domain.Entities;

namespace Shop_Cam_BE.Application.Features.Home.Queries.GetPromoBanners;

/// <summary>
/// Xử lý truy vấn lấy danh sách banner khuyến mãi (cột bên phải).
/// </summary>
public class GetPromoBannersQueryHandler : IRequestHandler<GetPromoBannersQuery, List<HomeBanner>>
{
    private readonly IApplicationDbContext _context;

    public GetPromoBannersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<HomeBanner>> Handle(GetPromoBannersQuery request, CancellationToken cancellationToken)
    {
        // Chỉ lấy các banner không phải banner chính (IsMain = false),
        // dùng cho vùng khuyến mãi bên phải trên trang chủ.
        return await _context.HomeBanners
            .Where(b => !b.IsMain)
            .OrderBy(b => b.DisplayOrder)
            .ToListAsync(cancellationToken);
    }
}

