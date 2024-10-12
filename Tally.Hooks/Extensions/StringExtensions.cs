using System.Text.RegularExpressions;

namespace Tally.Hooks.Extensions;

public static class StringExtensions
{
    public static string ToCapitalCase(this string? text)
    {
        var result = Regex.Replace(text ?? string.Empty, @"\b(\w)", m => m.Value.ToUpper());
        return Regex.Replace(result, @"(\s(of|in|by|and)|\'[st])\b", m => m.Value.ToLower(), RegexOptions.IgnoreCase);
    }
}

