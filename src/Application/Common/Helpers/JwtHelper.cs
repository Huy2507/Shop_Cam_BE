using System.Text;
using System.Text.Json;

namespace Shop_Cam_BE.Application.Common.Helpers;

public static class JwtHelper
{
    /// <summary>Đọc claim <c>sub</c> từ JWT (payload) không verify chữ ký — chỉ dùng khi cần đọc cookie nhanh.</summary>
    public static string? ExtractSubFromJwt(string jwtToken)
    {
        try
        {
            var document = ParsePayloadDocument(jwtToken);
            return document?.RootElement.GetProperty("sub").GetString();
        }
        catch
        {
            return null;
        }
    }

    private static JsonDocument? ParsePayloadDocument(string jwtToken)
    {
        try
        {
            var parts = jwtToken.Split('.');
            if (parts.Length < 2)
                return null;
            var payload = parts[1];
            var base64 = payload.Replace('-', '+').Replace('_', '/');
            var padded = base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=');
            var bytes = Convert.FromBase64String(padded);
            var json = Encoding.UTF8.GetString(bytes);
            return JsonDocument.Parse(json);
        }
        catch
        {
            return null;
        }
    }
}
