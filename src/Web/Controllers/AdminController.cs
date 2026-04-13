using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop_Cam_BE.Application.Common.Constants;
using Shop_Cam_BE.Application.Features.Admin.Queries.GetAdminSummary;
using Shop_Cam_BE.Application.Features.SiteSettings.Commands.UpsertSiteSettings;
using Shop_Cam_BE.Application.Features.SiteSettings.Queries.GetSiteSettingsByGroup;

namespace Shop_Cam_BE.Web.Controllers;

/// <summary>
/// API quản trị: tổng quan và cấu hình site (cần JWT hợp lệ).
/// </summary>
[ApiController]
[Route("api/admin")]
[Authorize(Policy = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Số liệu đếm nhanh cho dashboard admin.
    /// </summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var dto = await _mediator.Send(new GetAdminSummaryQuery(), cancellationToken);
        return Ok(dto);
    }

    /// <summary>
    /// Danh sách cấu hình theo nhóm (vd: UI).
    /// </summary>
    [HttpGet("site-settings")]
    public async Task<IActionResult> GetSiteSettings([FromQuery] string group, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(group))
            return BadRequest(new { message = "Thiếu tham số group." });

        var list = await _mediator.Send(new GetSiteSettingsByGroupQuery { Group = group }, cancellationToken);
        return Ok(list);
    }

    /// <summary>
    /// Ghi đè / tạo nhiều key cấu hình trong một lần (whitelist theo Application).
    /// </summary>
    [HttpPut("site-settings")]
    public async Task<IActionResult> UpsertSiteSettings([FromBody] UpsertSiteSettingsCommand command, CancellationToken cancellationToken)
    {
        if (command?.Items == null || command.Items.Count == 0)
            return BadRequest(new { errorCode = ErrorCodes.EMPTY_SITE_SETTINGS_ITEMS });

        var result = await _mediator.Send(command, cancellationToken);
        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }
}
