namespace Shop_Cam_BE.Application.DTOs;

public class CurrentUserDto
{
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string[] Roles { get; set; } = Array.Empty<string>();
}
