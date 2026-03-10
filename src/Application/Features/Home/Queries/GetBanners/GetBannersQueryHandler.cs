using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Domain.Entities;

namespace Shop_Cam_BE.Application.Features.Home.Queries.GetBanners;

/// <summary>
/// Xử lý truy vấn lấy danh sách banner chính.
/// Tách toàn bộ truy vấn DB ra khỏi Controller theo chuẩn CQRS.
/// </summary>
public class GetBannersQueryHandler : IRequestHandler<GetBannersQuery, List<HomeBanner>>
{
    private readonly IApplicationDbContext _context;

    public GetBannersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<HomeBanner>> Handle(GetBannersQuery request, CancellationToken cancellationToken)
    {
        // Lấy toàn bộ banner, sắp xếp theo thứ tự hiển thị.
        // Phần map sang DTO/ẩn bớt field sẽ được thực hiện ở Web layer.
        return await _context.HomeBanners
            .OrderBy(b => b.DisplayOrder)
            .ToListAsync(cancellationToken);
    }
}

