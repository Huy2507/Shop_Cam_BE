using MediatR;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Auth.Commands.ForgotPassword;

/// <summary>
/// Yêu cầu gửi OTP đặt lại mật khẩu (email + accessFrom); kiểm tra role theo RoleAccess.
/// </summary>
public record ForgotPasswordCommand(ForgotPasswordDto Dto) : IRequest<Result<Unit>>;
