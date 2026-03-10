using MediatR;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Auth.Commands.ForgotPassword;

public record ForgotPasswordCommand(ForgotPasswordDto Dto) : IRequest<Result<Unit>>;
