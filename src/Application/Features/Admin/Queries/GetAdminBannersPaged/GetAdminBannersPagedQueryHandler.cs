using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Helpers;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Admin.Queries.GetAdminBannersPaged;

/// <summary>
/// Đọc HomeBanners, sắp theo DisplayOrder rồi phân trang.
/// </summary>
public class GetAdminBannersPagedQueryHandler : IRequestHandler<GetAdminBannersPagedQuery, PagedResult<AdminBannerListItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAdminBannersPagedQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<AdminBannerListItemDto>> Handle(
        GetAdminBannersPagedQuery request,
        CancellationToken cancellationToken)
    {
        var page = AdminPagination.NormalizePage(request.Page);
        var pageSize = AdminPagination.NormalizePageSize(request.PageSize);
        var search = request.Search?.Trim();

        var q = _context.HomeBanners.AsNoTracking();
        if (!string.IsNullOrEmpty(search))
        {
            q = q.Where(b =>
                b.UrlImg.Contains(search) ||
                (b.Title != null && b.Title.Contains(search)));
        }

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderBy(b => b.DisplayOrder)
            .ThenBy(b => b.HomeBannerId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new AdminBannerListItemDto
            {
                HomeBannerId = b.HomeBannerId,
                UrlImg = b.UrlImg,
                Title = b.Title,
                Link = b.Link,
                IsMain = b.IsMain,
                DisplayOrder = b.DisplayOrder,
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<AdminBannerListItemDto>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize,
        };
    }
}
