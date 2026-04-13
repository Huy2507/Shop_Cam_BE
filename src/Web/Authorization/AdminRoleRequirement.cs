using Microsoft.AspNetCore.Authorization;

namespace Shop_Cam_BE.Web.Authorization;

/// <summary>Yêu cầu user đã đăng nhập JWT và có role Admin trong DB.</summary>
public sealed class AdminRoleRequirement : IAuthorizationRequirement
{
}
