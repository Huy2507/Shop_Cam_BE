using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Shop_Cam_BE.Application.Common.Constants;
using Shop_Cam_BE.Application.Common.Helpers;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;
using Shop_Cam_BE.Domain.Entities;

namespace Shop_Cam_BE.Application.Features.Auth.Commands.LoginForAdmin;

/// <summary>Đăng nhập admin: mật khẩu cục bộ + JWT; quyền Admin từ UserRoles.</summary>
public class LoginForAdminHandler : IRequestHandler<LoginForAdminCommand, Result<TokenResultDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IJwtTokenService _jwt;
    private readonly ILogger<LoginForAdminHandler> _logger;

    public LoginForAdminHandler(
        IApplicationDbContext context,
        IPasswordHasher<User> passwordHasher,
        IJwtTokenService jwt,
        ILogger<LoginForAdminHandler> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwt = jwt;
        _logger = logger;
    }

    public async Task<Result<TokenResultDto>> Handle(LoginForAdminCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await AuthUserQueries.FindByUsernameOrEmailAsync(_context, request.Username, cancellationToken);
            if (user == null || string.IsNullOrEmpty(user.PasswordHash))
                return Result<TokenResultDto>.Failure(ErrorCodes.INVALID_CREDENTIALS);

            if (_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
                return Result<TokenResultDto>.Failure(ErrorCodes.INVALID_CREDENTIALS);

            if (!user.IsActive)
                return Result<TokenResultDto>.Failure(ErrorCodes.ACCOUNT_LOCKED);

            if (!await UserRoleChecker.UserHasAdminRoleAsync(_context, user.UserId, cancellationToken))
                return Result<TokenResultDto>.Failure(ErrorCodes.UNAUTHORIZED_ROLE);

            var roleNames = await AuthUserQueries.GetRoleNamesAsync(_context, user.UserId, cancellationToken);
            return Result<TokenResultDto>.Success(_jwt.CreateTokenPair(user, roleNames));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi đăng nhập Admin cho {Username}", request.Username);
            return Result<TokenResultDto>.Failure(ErrorCodes.SERVER_ERROR);
        }
    }
}
