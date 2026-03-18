using System.Text.Json;
using System.Text.Json.Serialization;
using AiryBotCode.Domain.database;
using System.Net.Http.Json;
using System.Text;
using AiryBotCode.Application.Interfaces.Service;

namespace AiryBotCode.Application.Services.AIService
{
    public class OllamaClient : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _modelName;

        public OllamaClient(string baseUrl, string modelName)
        {
            _modelName = modelName;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        public async Task<string> SendMessageAsync(List<Message> messages)
        {
            var ollamaMessages = messages.Select(msg =>
            {
                var role = msg.User.Role.ToString().ToLower();
                return new
                {
                    role = role == "system" ? "system" : (role == "user" ? "user" : "assistant"),
                    content = msg.Context
                };
            }).ToList();

            var requestBody = new
            {
                model = _modelName,
                messages = ollamaMessages,
                stream = false
            };

            var response = await _httpClient.PostAsJsonAsync("api/chat", requestBody);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OllamaResponse>();
            return result?.Message?.Content ?? string.Empty;
        }

        private class OllamaResponse
        {
            [JsonPropertyName("model")]
            public string Model { get; set; }

            [JsonPropertyName("message")]
            public MessageData Message { get; set; }

            public class MessageData
            {
                [JsonPropertyName("role")]
                public string Role { get; set; }

                [JsonPropertyName("content")]
                public string Content { get; set; }
            }
        }
    }
}
