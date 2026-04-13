using MediatR;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Auth.Commands.RefreshToken;

/// <summary>
/// Làm mới cặp token từ cookie refresh_token; accessFrom xác định luồng (admin / app / …).
/// </summary>
public record RefreshTokenCommand(string? AccessFrom) : IRequest<Result<TokenResultDto>>;
