using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.SiteSettings.Queries.GetSiteSettingsByGroup;

/// <summary>
/// Lọc SiteSettings theo Group, sắp theo Key.
/// </summary>
public class GetSiteSettingsByGroupQueryHandler : IRequestHandler<GetSiteSettingsByGroupQuery, List<SiteSettingItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSiteSettingsByGroupQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SiteSettingItemDto>> Handle(GetSiteSettingsByGroupQuery request, CancellationToken cancellationToken)
    {
        var g = request.Group.Trim();
        return await _context.SiteSettings
            .AsNoTracking()
            .Where(x => x.Group == g)
            .OrderBy(x => x.Key)
            .Select(x => new SiteSettingItemDto
            {
                Group = x.Group,
                Key = x.Key,
                ValueJson = x.ValueJson,
                Description = x.Description,
                UpdatedAt = x.UpdatedTime,
            })
            .ToListAsync(cancellationToken);
    }
}
