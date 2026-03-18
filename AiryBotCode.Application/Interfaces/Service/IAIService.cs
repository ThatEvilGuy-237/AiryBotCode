using AiryBotCode.Domain.database;

namespace AiryBotCode.Application.Interfaces.Service
{
    public interface IAIService
    {
        Task<string> SendMessageAsync(List<Message> messages);
        // Future-proofing for tool calling support in the agent loop
        // Task<AIResult> SendMessageWithToolsAsync(List<Message> messages, List<ToolDefinition> tools);
    }
}
