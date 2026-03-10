using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Domain.Entities;

namespace Shop_Cam_BE.Application.Features.Home.Queries.GetNews;

/// <summary>
/// Xử lý truy vấn lấy danh sách tin tức hiển thị trên trang chủ.
/// </summary>
public class GetNewsQueryHandler : IRequestHandler<GetNewsQuery, List<NewsArticle>>
{
    private readonly IApplicationDbContext _context;

    public GetNewsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<NewsArticle>> Handle(GetNewsQuery request, CancellationToken cancellationToken)
    {
        // Lấy các bài viết mới nhất, sắp xếp theo PublishedAt giảm dần.
        return await _context.NewsArticles
            .OrderByDescending(n => n.PublishedAt)
            .Take(10)
            .ToListAsync(cancellationToken);
    }
}

