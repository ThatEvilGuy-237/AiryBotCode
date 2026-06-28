using System.Security.Cryptography;
using System.Text;

namespace AiryBotCode.Application.Services
{
    /// <summary>
    /// Pure helpers for the suggestions-board capability code. The code is the only
    /// credential for the public share link, so it must be unguessable and compared
    /// in constant time (no early-out on the first differing char).
    /// </summary>
    public static class ShareCode
    {
        // 256 bits of entropy, URL-safe base64 (no +, /, =) so it drops straight into a
        // path segment. ~43 chars — not guessable, not enumerable.
        public static string Generate()
        {
            Span<byte> bytes = stackalloc byte[32];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        // Constant-time compare so an attacker can't time-probe the code character by
        // character. Empty/null on either side is always a non-match.
        public static bool Matches(string? stored, string? provided)
        {
            if (string.IsNullOrEmpty(stored) || string.IsNullOrEmpty(provided)) return false;
            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(stored),
                Encoding.UTF8.GetBytes(provided));
        }
    }
}
