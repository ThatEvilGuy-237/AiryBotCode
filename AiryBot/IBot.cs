using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.AiryBot
{
    public interface IBot
    {
        Task StartAsync(IServiceProvider services);
        Task StopAsync();
    }
}
