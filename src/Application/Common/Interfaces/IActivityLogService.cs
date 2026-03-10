using Shop_Cam_BE.Domain.Enums;

namespace Shop_Cam_BE.Application.Common.Interfaces;

public interface IActivityLogService
{
    Task LogAsync(ActivityAction action, Guid userId, object? metadata = null);
}
