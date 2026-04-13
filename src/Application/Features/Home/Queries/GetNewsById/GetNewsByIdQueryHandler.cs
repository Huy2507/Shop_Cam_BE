using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Home.Queries.GetNewsById;

/// <summary>
/// Map NewsArticles sang NewsDetailDto.
/// </summary>
public class GetNewsByIdQueryHandler : IRequestHandler<GetNewsByIdQuery, NewsDetailDto?>
{
    private readonly IApplicationDbContext _context;

    public GetNewsByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<NewsDetailDto?> Handle(GetNewsByIdQuery request, CancellationToken cancellationToken)
    {
        var n = await _context.NewsArticles
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.NewsArticleId == request.NewsArticleId && x.IsActive,
                cancellationToken);

        if (n == null) return null;

        return new NewsDetailDto
        {
            Id = n.NewsArticleId,
            Title = n.Title,
            ImageUrl = n.ImageUrl,
            Excerpt = n.Excerpt,
            Body = n.Body,
            PublishedAt = n.PublishedAt
        };
    }
}
