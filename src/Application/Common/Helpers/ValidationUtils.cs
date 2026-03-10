using System.Text.RegularExpressions;

namespace Shop_Cam_BE.Application.Common.Helpers;

public static class ValidationUtils
{
    public static bool IsValidEmail(string input)
    {
        return Regex.IsMatch(input, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
    }
}
