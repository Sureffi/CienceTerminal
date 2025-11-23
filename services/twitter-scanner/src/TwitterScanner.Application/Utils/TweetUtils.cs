using System.Text.RegularExpressions;

namespace TwitterScanner.Application.Utils;

/// <summary>
/// Utility functions for extracting SPL token addresses from tweets
/// or formatting values
/// </summary>
public static class TweetUtils
{
    // Base58 character set used by Solana
    private static readonly string Base58Chars = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

    // Regex pattern for potential Solana addresses (32-44 characters, base58)
    private static readonly Regex SolanaAddressRegex = new Regex(
        $@"\b[{Base58Chars}]{{32,44}}\b",
        RegexOptions.Compiled
    );

    /// <summary>
    /// Extracts potential solana addresses from a tweets text
    /// </summary>
    /// <param name="tweetText"></param>
    /// <returns></returns>
    public static List<string> TryExtractCa(string tweetText)
    {
        if (string.IsNullOrEmpty(tweetText))
            return new List<string>();

        var matches = SolanaAddressRegex.Matches(tweetText);

        var addresses = new List<string>();
        foreach (Match match in matches)
        {
            string candidate = match.Value;
            if (IsValidSolanaAddress(candidate))
            {
                addresses.Add(candidate);
            }
        }

        return addresses.Distinct().ToList();
    }

    /// <summary>
    /// Basic validation for strings that match characteristics of a valid SPL token address
    /// </summary>
    /// <param name="address"></param>
    /// <returns></returns>
    private static bool IsValidSolanaAddress(string address)
    {
        // Check length (Solana addresses are typically 32-44 characters)
        if (address.Length < 32 || address.Length > 44)
            return false;

        // Check if all characters are valid base58
        if (!address.All(c => Base58Chars.Contains(c)))
            return false;

        // Exclude common false positives
        if (IsLikelyFalsePositive(address))
            return false;

        return true;
    }

    /// <summary>
    /// Basic check for if a string is likely a false positive SPL token address
    /// Checks for:
    /// - Too few unique characters
    /// - Common false positive string (All 1s, SOL, WSOL)
    /// </summary>
    /// <param name="address"></param>
    /// <returns></returns>
    private static bool IsLikelyFalsePositive(string address)
    {
        // Skip if it's mostly the same character repeated
        var uniqueChars = address.Distinct().Count();
        if (uniqueChars < 8) // Too few unique characters
            return true;

        // Common false positives and coins that shouldnt be counted
        var commonFalsePositives = new[]
        {
            "1111111111111111111111111111111111111111111", // All 1s
            "So11111111111111111111111111111111111111112", // Wrapped SOL
            "So11111111111111111111111111111111111111111", // SOL
        };

        return commonFalsePositives.Contains(address);
    }

    // Regex pattern to match cashtags: $ followed by 1-6 letters/numbers
    private static readonly Regex CashtagPattern = new Regex(@"\$[a-zA-Z][a-zA-Z0-9]{0,5}\b", RegexOptions.Compiled);

    /// <summary>
    /// Checks if the text contains any cashtags
    /// </summary>
    /// <param name="text">The tweet text to check</param>
    /// <returns>True if cashtags are found, false otherwise</returns>
    public static bool ContainsCashtags(string text)
    {
        if (string.IsNullOrEmpty(text))
            return false;

        return CashtagPattern.IsMatch(text);
    }

    /// <summary>
    /// Formats a number to display in k (thousands) or m (millions) format
    /// </summary>
    /// <param name="value">The numeric value to format</param>
    /// <returns>Formatted string with k or m suffix</returns>
    public static string FormatNumber(double value)
    {
        if (value >= 1_000_000)
        {
            return $"{value / 1_000_000:F1}m";
        }
        else if (value >= 1_000)
        {
            return $"{value / 1_000:F1}k";
        }
        else
        {
            return value.ToString("F2");
        }
    }
}
