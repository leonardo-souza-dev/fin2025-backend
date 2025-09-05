using System.Text.RegularExpressions;

namespace Fin.Api.Infrastructure;

public static class StringExtensions
{
    public static string ToKebabCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;
        
        return Regex.Replace(value, "([a-z])([A-Z])", "$1-$2", RegexOptions.None, TimeSpan.FromMilliseconds(100)).ToLower();
    }
}