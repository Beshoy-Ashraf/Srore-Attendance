using System.Text.RegularExpressions;

namespace Application.Common;

public static class MacAddress
{
    /// <summary>AA:BB:CC:DD:EE:FF or AA-BB-CC-DD-EE-FF, any case.</summary>
    public const string Pattern = "^([0-9A-Fa-f]{2}[:-]){5}[0-9A-Fa-f]{2}$";

    private static readonly Regex Regex = new(Pattern, RegexOptions.Compiled);

    public static bool IsValid(string? mac) => !string.IsNullOrWhiteSpace(mac) && Regex.IsMatch(mac.Trim());

    /// <summary>Canonical form used for storage and comparison: upper case, colon separated.</summary>
    public static string Normalize(string? mac) =>
        string.IsNullOrWhiteSpace(mac) ? string.Empty : mac.Trim().Replace("-", ":").ToUpperInvariant();
}
