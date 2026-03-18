namespace AiryBotCode.Application.Interfaces.Service
{
    public interface IInterpreterService
    {
        Task<string> ExecuteCommandAsync(string command);
        Task<string> ReadFileAsync(string path);
        Task<string> WriteFileAsync(string path, string content);
    }
}
