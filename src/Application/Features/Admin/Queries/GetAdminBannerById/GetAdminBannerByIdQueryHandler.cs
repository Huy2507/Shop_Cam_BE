using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Admin.Queries.GetAdminBannerById;

/// <summary>
/// Trả một dòng HomeBanners hoặc null.
/// </summary>
public class GetAdminBannerByIdQueryHandler : IRequestHandler<GetAdminBannerByIdQuery, AdminBannerListItemDto?>
{
    private readonly IApplicationDbContext _context;

    public GetAdminBannerByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AdminBannerListItemDto?> Handle(GetAdminBannerByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.HomeBanners
            .AsNoTracking()
            .Where(b => b.HomeBannerId == request.BannerId)
            .Select(b => new AdminBannerListItemDto
            {
                HomeBannerId = b.HomeBannerId,
                UrlImg = b.UrlImg,
                Title = b.Title,
                Link = b.Link,
                IsMain = b.IsMain,
                DisplayOrder = b.DisplayOrder,
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
