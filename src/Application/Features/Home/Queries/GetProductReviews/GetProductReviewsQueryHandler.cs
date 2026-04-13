using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Helpers;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Home.Queries.GetProductReviews;

/// <summary>
/// Trả rỗng nếu ProductId không tồn tại; ngược lại đếm và lấy trang ProductReviews (IsApproved).
/// </summary>
public class GetProductReviewsQueryHandler : IRequestHandler<GetProductReviewsQuery, ProductReviewsResult>
{
    private readonly IApplicationDbContext _context;

    public GetProductReviewsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductReviewsResult> Handle(GetProductReviewsQuery request, CancellationToken cancellationToken)
    {
        var exists = await _context.Products.AnyAsync(
            p => p.ProductId == request.ProductId && p.IsActive,
            cancellationToken);
        if (!exists)
            return new ProductReviewsResult { Page = 1, PageSize = request.PageSize };

        var page = StorefrontPagination.NormalizePage(request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 50);

        var baseQuery = _context.ProductReviews.AsNoTracking()
            .Where(r => r.ProductId == request.ProductId && r.IsApproved && r.IsActive);

        var total = await baseQuery.CountAsync(cancellationToken);

        double? avg = null;
        if (total > 0)
            avg = await baseQuery.AverageAsync(r => (double)r.Rating, cancellationToken);

        var items = await baseQuery
            .OrderByDescending(r => r.CreatedTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new ProductReviewDto
            {
                Id = r.ProductReviewId,
                AuthorName = r.AuthorName,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedTime.UtcDateTime,
            })
            .ToListAsync(cancellationToken);

        return new ProductReviewsResult
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize,
            AverageRating = avg,
        };
    }
}
