using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Interfaces
{
    public interface IConfigurationReader
    {
        void LoadConfig(string? path = null);
        public string GetBotToken();
    }
}