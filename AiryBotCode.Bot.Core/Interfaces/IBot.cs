namespace AiryBotCode.Bot.Core.Interfaces
{
    public interface IBot
    {
        Task StartAsync(IServiceProvider services);
        Task StopAsync();
    }
}
