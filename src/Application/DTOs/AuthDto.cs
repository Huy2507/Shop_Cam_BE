namespace Shop_Cam_BE.Application.DTOs;

public class LoginDTO
{
    public string username { get; set; } = null!;
    public string password { get; set; } = null!;
}

public class UserInfoDto
{
    public string KeycloakId { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
}

public class LoginResultDto
{
    public string Email { get; set; } = string.Empty;
}

public class TokenResultDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class ForgotPasswordDto
{
    public string Email { get; set; } = default!;
    public string? AccessFrom { get; set; }
}

public class VerifyResetCodeDto
{
    public required string Email { get; set; }
    public required string Code { get; set; }
    public string? AccessFrom { get; set; }
}

public class VerifyResetCodeResponse
{
    public bool IsValid { get; set; }
    public Guid UserId { get; set; }
}

public class ResetPasswordDto
{
    public required string Email { get; set; }
    public required string Code { get; set; }
    public required string NewPassword { get; set; }
    public required string ConfirmPassword { get; set; }
    public string? AccessFrom { get; set; }
}

public class KeycloakUserWithRoles
{
    public Guid UserId { get; set; }
    public List<string> Roles { get; set; } = new();
}
