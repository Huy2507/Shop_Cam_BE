using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop_Cam_BE.Application.DTOs;
using Shop_Cam_BE.Application.Features.Users.Commands.UpsertUserNewsPagePreference;
using Shop_Cam_BE.Application.Features.Users.Queries.GetCurrentUser;

namespace Shop_Cam_BE.Web.Controllers;

/// <summary>
/// Thông tin người dùng đã đăng nhập (cookie JWT).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Trả về user hiện tại từ access_token — dùng cho guard admin/storefront.
    /// </summary>
    [HttpGet("current-user")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCurrentUserQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Lưu tùy chọn hiển thị trang tin (layout, page size, …) cho user đăng nhập; storefront đọc qua GET ui-config.
    /// </summary>
    [HttpPut("me/news-page-preference")]
    [Authorize]
    public async Task<IActionResult> UpsertNewsPagePreference(
        [FromBody] UpsertUserNewsPagePreferenceDto? body,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpsertUserNewsPagePreferenceCommand { Body = body ?? new UpsertUserNewsPagePreferenceDto() },
            cancellationToken);
        if (!result.Succeeded)
            return BadRequest(new { errorCode = result.ErrorCode, errors = result.Errors });
        return NoContent();
    }
}
