using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Constants;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Domain.Entities;

namespace Shop_Cam_BE.Application.Features.SiteSettings.Commands.UpsertSiteSettings;

/// <summary>
/// Áp dụng từng item: kiểm tra khóa được phép; thời gian/người sửa do DbContext ghi khi SaveChanges.
/// </summary>
public class UpsertSiteSettingsCommandHandler : IRequestHandler<UpsertSiteSettingsCommand, Result<Unit>>
{
    private readonly IApplicationDbContext _context;

    public UpsertSiteSettingsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Unit>> Handle(UpsertSiteSettingsCommand request, CancellationToken cancellationToken)
    {
        foreach (var item in request.Items)
        {
            if (!SiteSettingGroups.IsAllowedKey(item.Group, item.Key))
                return Result<Unit>.Failure(ErrorCodes.SITE_SETTING_KEY_NOT_ALLOWED);

            if (string.IsNullOrWhiteSpace(item.ValueJson))
                return Result<Unit>.Failure(ErrorCodes.INVALID_DATA);

            var existing = await _context.SiteSettings
                .FirstOrDefaultAsync(
                    x => x.Group == item.Group && x.Key == item.Key,
                    cancellationToken);

            if (existing == null)
            {
                _context.SiteSettings.Add(new SiteSetting
                {
                    SiteSettingId = Guid.NewGuid(),
                    Group = item.Group,
                    Key = item.Key,
                    ValueJson = item.ValueJson.Trim(),
                });
            }
            else
            {
                existing.ValueJson = item.ValueJson.Trim();
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result<Unit>.Success(Unit.Value);
    }
}
