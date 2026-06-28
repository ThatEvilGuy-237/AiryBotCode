using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Application.Interfaces.Service;

namespace AiryBotCode.Application.Services
{
    /// <summary>
    /// The suggestions-board capability code, backed by the single-row store. Generation
    /// + comparison live in <see cref="ShareCode"/>; this only adds persistence.
    /// </summary>
    public class ShareCodeService : IShareCodeService
    {
        private readonly ISuggestionShareCodeRepository _repository;

        public ShareCodeService(ISuggestionShareCodeRepository repository) => _repository = repository;

        public async Task<string> GetOrCreateAsync()
        {
            var existing = await _repository.GetAsync();
            if (existing is not null && !string.IsNullOrEmpty(existing.Code)) return existing.Code;

            var code = ShareCode.Generate();
            await _repository.SetAsync(code);
            return code;
        }

        public async Task<string> RegenerateAsync()
        {
            var code = ShareCode.Generate();
            await _repository.SetAsync(code);
            return code;
        }

        public async Task<bool> ValidateAsync(string? provided)
        {
            if (string.IsNullOrEmpty(provided)) return false;
            var stored = await _repository.GetAsync();
            return stored is not null && ShareCode.Matches(stored.Code, provided);
        }
    }
}
