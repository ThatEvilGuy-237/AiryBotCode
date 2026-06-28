namespace AiryBotCode.Application.Interfaces.Service
{
    /// <summary>
    /// Manages the suggestions-board capability code: the dashboard reads/regenerates
    /// it, the public endpoints validate it on every request.
    /// </summary>
    public interface IShareCodeService
    {
        // Current code, generating + persisting one the first time.
        Task<string> GetOrCreateAsync();

        // Mint a fresh code (invalidates the previous share link). Returns the new code.
        Task<string> RegenerateAsync();

        // True only if `provided` matches the stored code (constant-time). Null/empty → false.
        Task<bool> ValidateAsync(string? provided);
    }
}
