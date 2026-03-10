using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shop_Cam_BE.Application.Common.Constants;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Infrastructure.Services;

public class KeycloakService : IKeycloakService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<KeycloakService> _logger;

    public KeycloakService(HttpClient httpClient, IConfiguration config, ILogger<KeycloakService> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }

    public async Task<Result<TokenResultDto>> LoginAsync(string username, string password)
    {
        var clientId = _config["Keycloak:ClientId"]
            ?? throw new InvalidOperationException("ClientId is not configured");
        var realm = _config["Keycloak:Realm"]
            ?? throw new InvalidOperationException("Realm is not configured");
        var tokenUrl = $"{_config["Keycloak:BaseUrl"]}/realms/{realm}/protocol/openid-connect/token";

        var requestData = new Dictionary<string, string>
        {
            ["client_id"] = clientId,
            ["client_secret"] = _config["Keycloak:ClientSecret"] ?? throw new InvalidOperationException("ClientSecret is not configured"),
            ["grant_type"] = "password",
            ["username"] = username,
            ["password"] = password
        };

        var content = new FormUrlEncodedContent(requestData);
        var response = await _httpClient.PostAsync(tokenUrl, content);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Login failed: {Content}", responseContent);
            return Result<TokenResultDto>.Failure(ErrorCodes.INVALID_CREDENTIALS, ["Tài khoản hoặc mật khẩu không đúng."]);
        }

        try
        {
            using var jsonDoc = JsonDocument.Parse(responseContent);
            var root = jsonDoc.RootElement;
            var accessToken = root.GetProperty("access_token").GetString();
            var refreshToken = root.GetProperty("refresh_token").GetString();
            var tokenDto = new TokenResultDto
            {
                AccessToken = accessToken!,
                RefreshToken = refreshToken!
            };
            return Result<TokenResultDto>.Success(tokenDto);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse login response");
            return Result<TokenResultDto>.Failure(ErrorCodes.SERVER_ERROR, ["Invalid response from identity server."]);
        }
    }

    public async Task<Result<TokenResultDto>> RefreshTokenAsync(string refreshToken)
    {
        var baseUrl = _config["Keycloak:BaseUrl"]?.TrimEnd('/');
        var clientId = _config["Keycloak:ClientId"] ?? throw new InvalidOperationException("ClientId is not configured");
        var realm = _config["Keycloak:Realm"] ?? throw new InvalidOperationException("Realm is not configured");
        var clientSecret = _config["Keycloak:ClientSecret"] ?? throw new InvalidOperationException("ClientSecret is not configured");
        var tokenUrl = $"{baseUrl}/realms/{realm}/protocol/openid-connect/token";

        var form = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", refreshToken },
            { "client_id", clientId },
            { "client_secret", clientSecret }
        };

        var response = await _httpClient.PostAsync(tokenUrl, new FormUrlEncodedContent(form));
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Refresh token failed: {StatusCode} {Content}", response.StatusCode, responseContent);
            return Result<TokenResultDto>.Failure(ErrorCodes.TOKEN_EXPIRED, ["Làm mới token thất bại"]);
        }

        var doc = JsonDocument.Parse(responseContent).RootElement;
        var token = new TokenResultDto
        {
            AccessToken = doc.GetProperty("access_token").GetString()!,
            RefreshToken = doc.GetProperty("refresh_token").GetString()!,
        };
        return Result<TokenResultDto>.Success(token);
    }

    public async Task<bool> LogoutUserAsync(string refreshToken)
    {
        var baseUrl = _config["Keycloak:BaseUrl"];
        var clientId = _config["Keycloak:ClientId"] ?? throw new InvalidOperationException("ClientId is not configured");
        var realm = _config["Keycloak:Realm"] ?? throw new InvalidOperationException("Realm is not configured");
        var clientSecret = _config["Keycloak:ClientSecret"] ?? throw new InvalidOperationException("ClientSecret is not configured");
        var parameters = new Dictionary<string, string>
        {
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "refresh_token", refreshToken }
        };
        var content = new FormUrlEncodedContent(parameters);
        var response = await _httpClient.PostAsync($"{baseUrl}/realms/{realm}/protocol/openid-connect/logout", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<Result<Unit>> ResetPasswordAsync(Guid userId, string newPassword)
    {
        try
        {
            var url = $"{_config["Keycloak:BaseUrl"]}/admin/realms/{_config["Keycloak:Realm"]}/users/{userId}/reset-password";
            var request = new HttpRequestMessage(HttpMethod.Put, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await GetAdminTokenAsync());
            var payload = new { type = "password", value = newPassword, temporary = false };
            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
                return Result<Unit>.Success(Unit.Value);

            var body = await response.Content.ReadAsStringAsync();
            return Result<Unit>.Failure(ErrorCodes.SERVER_ERROR, ["Failed to reset password", body]);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Password reset error");
            return Result<Unit>.Failure(ErrorCodes.SERVER_ERROR, [$"Password reset error: {ex.Message}"]);
        }
    }

    public async Task<Result<KeycloakUserWithRoles>> GetUserIdAndRolesByEmailAsync(string email)
    {
        try
        {
            var baseUrl = _config["Keycloak:BaseUrl"];
            var realm = _config["Keycloak:Realm"];
            var url = $"{baseUrl}/admin/realms/{realm}/users?email={Uri.EscapeDataString(email)}&exact=true";

            var adminToken = await GetAdminTokenAsync();
            if (string.IsNullOrEmpty(adminToken))
                return Result<KeycloakUserWithRoles>.Failure(ErrorCodes.SERVER_ERROR, ["Admin token missing"]);

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return Result<KeycloakUserWithRoles>.Failure(ErrorCodes.SERVER_ERROR, ["User lookup failed"]);

            var content = await response.Content.ReadAsStringAsync();
            var users = JsonSerializer.Deserialize<List<KeycloakUserDto>>(content);
            if (users is null || users.Count == 0)
                return Result<KeycloakUserWithRoles>.Failure(ErrorCodes.USER_NOT_FOUND, ["User not found"]);

            var user = users[0];
            if (!Guid.TryParse(user.Id, out var userId))
                return Result<KeycloakUserWithRoles>.Failure(ErrorCodes.SERVER_ERROR, ["Invalid userId"]);

            var roleUrl = $"{baseUrl}/admin/realms/{realm}/users/{user.Id}/role-mappings/realm";
            var roleRequest = new HttpRequestMessage(HttpMethod.Get, roleUrl);
            roleRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
            var roleResponse = await _httpClient.SendAsync(roleRequest);
            if (!roleResponse.IsSuccessStatusCode)
                return Result<KeycloakUserWithRoles>.Failure(ErrorCodes.SERVER_ERROR, ["Failed to get roles"]);

            var roleContent = await roleResponse.Content.ReadAsStringAsync();
            var roles = JsonSerializer.Deserialize<List<KeycloakRoleDto>>(roleContent)?
                .Where(r => !string.IsNullOrEmpty(r.Name)).Select(r => r.Name!).ToList() ?? new List<string>();

            return Result<KeycloakUserWithRoles>.Success(new KeycloakUserWithRoles { UserId = userId, Roles = roles });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user and roles by email");
            return Result<KeycloakUserWithRoles>.Failure(ErrorCodes.SERVER_ERROR, ["Internal error"]);
        }
    }

    public async Task<Result<UserInfoDto>> GetUserInfoByUsernameAsync(string username)
    {
        var realm = _config["Keycloak:Realm"];
        var baseUrl = _config["Keycloak:BaseUrl"];
        var token = await GetAdminTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var url = $"{baseUrl}/admin/realms/{realm}/users?username={username}";
        var response = await _httpClient.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            return Result<UserInfoDto>.Failure(ErrorCodes.USER_NOT_FOUND, ["Không tìm thấy thông tin người dùng"]);

        try
        {
            var users = JsonSerializer.Deserialize<List<JsonElement>>(content);
            var user = users?.FirstOrDefault();
            if (!user.HasValue || user.Value.ValueKind == JsonValueKind.Undefined)
                return Result<UserInfoDto>.Failure(ErrorCodes.USER_NOT_FOUND, ["Người dùng không tồn tại"]);

            var userElement = user.Value;
            var userInfo = new UserInfoDto
            {
                KeycloakId = userElement.GetProperty("id").GetString()!,
                Username = userElement.GetProperty("username").GetString()!,
                Email = userElement.GetProperty("email").GetString()!
            };
            return Result<UserInfoDto>.Success(userInfo);
        }
        catch
        {
            return Result<UserInfoDto>.Failure(ErrorCodes.SERVER_ERROR, ["Lỗi xử lý dữ liệu người dùng"]);
        }
    }

    public Task<bool> CheckUserHasAnyRoleAsync(string accessToken, IEnumerable<string> requiredRoles)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
            return Task.FromResult(false);

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(accessToken);
            var roles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var claim in token.Claims.Where(c => c.Type == "realm_access"))
            {
                using var doc = JsonDocument.Parse(claim.Value);
                if (doc.RootElement.TryGetProperty("roles", out var rolesElement))
                {
                    foreach (var role in rolesElement.EnumerateArray())
                    {
                        if (!string.IsNullOrWhiteSpace(role.GetString()))
                            roles.Add(role.GetString()!);
                    }
                }
            }

            var requiredSet = requiredRoles.Select(r => r.Trim()).Where(r => !string.IsNullOrEmpty(r)).ToHashSet(StringComparer.OrdinalIgnoreCase);
            return Task.FromResult(requiredSet.Count > 0 && roles.Overlaps(requiredSet));
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    private async Task<string> GetAdminTokenAsync()
    {
        var clientId = _config["Keycloak:ClientId"] ?? throw new InvalidOperationException("ClientId is not configured");
        var clientSecret = _config["Keycloak:ClientSecret"] ?? throw new InvalidOperationException("ClientSecret is not configured");
        var realm = _config["Keycloak:Realm"] ?? throw new InvalidOperationException("Realm is not configured");
        var tokenUrl = $"{_config["Keycloak:BaseUrl"]}/realms/{realm}/protocol/openid-connect/token";

        var requestData = new Dictionary<string, string>
        {
            ["client_id"] = clientId,
            ["client_secret"] = clientSecret,
            ["grant_type"] = "client_credentials"
        };

        var response = await _httpClient.PostAsync(tokenUrl, new FormUrlEncodedContent(requestData));
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Keycloak admin token failed: {Content}", content);
            throw new InvalidOperationException("Keycloak token request failed: " + content);
        }

        using var json = JsonDocument.Parse(content);
        if (!json.RootElement.TryGetProperty("access_token", out var tokenProp))
            throw new InvalidOperationException("Missing access_token in response");
        return tokenProp.GetString()!;
    }

    private record KeycloakUserDto([property: JsonPropertyName("id")] string Id, [property: JsonPropertyName("email")] string Email, [property: JsonPropertyName("username")] string Username);
    private record KeycloakRoleDto([property: JsonPropertyName("name")] string? Name);
}
