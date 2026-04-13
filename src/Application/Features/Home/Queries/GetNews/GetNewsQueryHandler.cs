using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Home.Queries.GetNews;

/// <summary>
/// Tin active, sắp theo PublishedAt giảm dần, phân trang.
/// </summary>
public class GetNewsQueryHandler : IRequestHandler<GetNewsQuery, NewsFeedResult>
{
    private readonly IApplicationDbContext _context;

    public GetNewsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<NewsFeedResult> Handle(GetNewsQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 50);

        var baseQuery = _context.NewsArticles
            .AsNoTracking()
            .Where(n => n.IsActive)
            .OrderByDescending(n => n.PublishedAt);

        var totalCount = await baseQuery.CountAsync(cancellationToken);
        var items = await baseQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new NewsFeedResult
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
        };
    }
}
