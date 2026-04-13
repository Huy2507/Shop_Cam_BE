using Microsoft.AspNetCore.Mvc;
using Shop_Cam_BE.Application.Common.Constants;
using Shop_Cam_BE.Application.DTOs;
using Shop_Cam_BE.Application.Features.Auth.Commands;
using Shop_Cam_BE.Application.Features.Auth.Commands.ForgotPassword;
using Shop_Cam_BE.Application.Features.Auth.Commands.Logout;
using Shop_Cam_BE.Application.Features.Auth.Commands.RefreshToken;
using Shop_Cam_BE.Application.Features.Auth.Commands.ResendOtp;
using Shop_Cam_BE.Application.Features.Auth.Commands.ResetPassword;
using Shop_Cam_BE.Application.Features.Auth.Commands.VerifyResetCode;
using Shop_Cam_BE.Application.Features.Auth.Commands.LoginForAdmin;
using Shop_Cam_BE.Application.Features.Auth.Commands.LoginForDashboard;
using MediatR;

namespace Shop_Cam_BE.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Đăng nhập trang quản trị: user/pass → JWT cục bộ; cookie HttpOnly; quyền Admin từ DB.</summary>
    [HttpPost("login/admin")]
    public async Task<IActionResult> LoginForAdmin([FromBody] LoginForAdminCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
            return BadRequest(result);

        Response.Cookies.Append("access_token", result.Value!.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddHours(8),
        });
        Response.Cookies.Append("refresh_token", result.Value!.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(30),
        });
        Response.Cookies.Append("username", command.Username, new CookieOptions
        {
            Expires = DateTime.UtcNow.AddHours(1),
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
        });
        Response.Cookies.Append("email", result.Value.Email, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddMinutes(5),
        });
        return Ok(result);
    }

    /// <summary>Đăng nhập dashboard (storefront): JWT cục bộ + cookie giống admin để refresh hoạt động.</summary>
    [HttpPost("login/dashboard")]
    public async Task<IActionResult> LoginForDashboard([FromBody] LoginForDashboardCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
            return BadRequest(result);

        Response.Cookies.Append("access_token", result.Value!.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddHours(8),
            Path = "/",
        });
        Response.Cookies.Append("refresh_token", result.Value!.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(30),
            Path = "/",
        });
        var cookieOptions = new CookieOptions
        {
            Expires = DateTime.UtcNow.AddHours(1),
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/",
        };
        Response.Cookies.Append("username", command.Username, cookieOptions);
        Response.Cookies.Append("email", result.Value.Email, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddMinutes(5),
            Path = "/",
        });
        return Ok(result);
    }

    [HttpPost("resend-otp")]
    public async Task<IActionResult> ResendOtp([FromBody] ResendOtpCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromQuery] string accessFrom = "app")
    {
        if (!Request.Cookies.TryGetValue("refresh_token", out _))
            return BadRequest(new { errorCode = ErrorCodes.INVALID_REFRESH_TOKEN });

        var result = await _mediator.Send(new RefreshTokenCommand(accessFrom));
        if (!result.Succeeded)
            return BadRequest(result);

        Response.Cookies.Append("access_token", result.Value!.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddHours(8),
            Path = "/"
        });
        Response.Cookies.Append("refresh_token", result.Value!.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(30),
            Path = "/"
        });
        return Ok(result.Value);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refresh_token"];
        await _mediator.Send(new LogoutCommand { RefreshToken = refreshToken ?? string.Empty });
        Response.Cookies.Delete("access_token");
        Response.Cookies.Delete("refresh_token");
        Response.Cookies.Delete("username");
        Response.Cookies.Delete("email");
        return Ok();
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        var result = await _mediator.Send(new ForgotPasswordCommand(dto));
        if (result.Succeeded)
            return Ok(new { success = true });
        return BadRequest(result);
    }

    [HttpPost("verify-reset-code")]
    public async Task<IActionResult> VerifyResetCode([FromBody] VerifyResetCodeDto request)
    {
        var result = await _mediator.Send(new VerifyResetCodeQuery
        {
            Email = request.Email,
            Code = request.Code,
            AccessFrom = request.AccessFrom,
        });
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var result = await _mediator.Send(new ResetPasswordCommand { Dto = dto });
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}
