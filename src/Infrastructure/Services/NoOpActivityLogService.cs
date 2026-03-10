using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Domain.Enums;

namespace Shop_Cam_BE.Infrastructure.Services;

public class NoOpActivityLogService : IActivityLogService
{
    public Task LogAsync(ActivityAction action, Guid userId, object? metadata = null)
    {
        return Task.CompletedTask;
    }
}
