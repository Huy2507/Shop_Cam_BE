using MediatR;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string? AccessFrom) : IRequest<Result<TokenResultDto>>;
