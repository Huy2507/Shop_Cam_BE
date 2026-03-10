using System.Text;
using System.Text.Json;

namespace Shop_Cam_BE.Application.Common.Helpers;

public static class JwtHelper
{
    public static string? ExtractKeycloakIdFromJwt(string jwtToken)
    {
        try
        {
            var payload = jwtToken.Split('.')[1];
            var base64 = payload.Replace('-', '+').Replace('_', '/');
            var padded = base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=');
            var bytes = Convert.FromBase64String(padded);
            var json = Encoding.UTF8.GetString(bytes);
            var document = JsonDocument.Parse(json);
            return document.RootElement.GetProperty("sub").GetString();
        }
        catch
        {
            return null;
        }
    }
}
