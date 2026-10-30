using System.Threading.RateLimiting;
using Evil.Sdk;

namespace AiryBotCode.Api;

public sealed record AiryDatabase(string ConnectionString);

public sealed record DiscordSignIn(bool On, string? ClientId, string? ClientSecret)
{
    public static readonly string[] Scopes = ["identify"];

    public OAuthApp App => new(ClientId ?? "", ClientSecret ?? "", Scopes);
}

public static class PublicSuggestLimit
{
    public const string Policy = "public-suggest-create";

    public static RateLimitPartition<string> For(HttpContext http) =>
        RateLimitPartition.GetFixedWindowLimiter(http.Connection.RemoteIpAddress?.ToString() ?? "unknown", Window);

    private static FixedWindowRateLimiterOptions Window(string _) => new() { PermitLimit = 5, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 };
}
