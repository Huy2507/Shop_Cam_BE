using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Admin.Queries.GetAdminSummary;

/// <summary>
/// Đếm song song Products, Orders, Users, Reviews, Categories, NewsArticles.
/// </summary>
public class GetAdminSummaryQueryHandler : IRequestHandler<GetAdminSummaryQuery, AdminSummaryDto>
{
    private readonly IApplicationDbContext _context;

    public GetAdminSummaryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AdminSummaryDto> Handle(GetAdminSummaryQuery request, CancellationToken cancellationToken)
    {
        return new AdminSummaryDto
        {
            ProductsCount = await _context.Products.CountAsync(p => p.IsActive, cancellationToken),
            OrdersCount = await _context.Orders.CountAsync(o => o.IsActive, cancellationToken),
            UsersCount = await _context.Users.CountAsync(u => u.IsActive, cancellationToken),
            ReviewsCount = await _context.ProductReviews.CountAsync(r => r.IsActive, cancellationToken),
            CategoriesCount = await _context.ProductCategories.CountAsync(c => c.IsActive, cancellationToken),
            NewsCount = await _context.NewsArticles.CountAsync(n => n.IsActive, cancellationToken),
        };
    }
}
