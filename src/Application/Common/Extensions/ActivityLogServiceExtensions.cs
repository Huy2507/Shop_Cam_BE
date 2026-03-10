using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Domain.Enums;

namespace Shop_Cam_BE.Application.Common.Extensions;

public static class ActivityLogServiceExtensions
{
    public static Task LogUserActionAsync(
        this IActivityLogService activityLogService,
        Guid userId,
        ActivityAction action,
        object? metadata = null)
    {
        return activityLogService.LogAsync(action, userId, metadata);
    }
}
