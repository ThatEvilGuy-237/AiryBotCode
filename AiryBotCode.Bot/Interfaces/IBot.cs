namespace AiryBotCode.Bot.Interfaces
{
    public interface IBot
    {
        Task StartAsync(IServiceProvider services);
        Task StopAsync();
    }
}
