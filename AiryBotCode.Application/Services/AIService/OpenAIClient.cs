using AiryBotCode.Domain.database;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace AiryBotCode.Application.Services.AIService
{
    public class OpenAIClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _modelName;
        private readonly string _apiKey;

        public OpenAIClient(string apiKey,
            string modelName = "gpt-4o-mini")
        {
            _modelName = modelName;
            _apiKey = apiKey;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.openai.com/v1/")
            };
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public async Task<string> SendMessageAsync(List<Message> messages)
        {
            var openAIMessages = messages.Select(msg => new
            {
                role = msg.User.Role.ToString().ToLower(), // Convert ChatRole enum to lowercase string
                content = msg.Context
            }).ToList();

            Console.WriteLine("[OpenAI] Prompt:");
            foreach (var msg in openAIMessages)
            {
                Console.WriteLine($"[{msg.role}]: {msg.content}");
            }

            var requestBody = new
            {
                model = _modelName,
                messages = openAIMessages, // Send the structured messages
                max_tokens = 800
            };

            HttpResponseMessage response;
            int retries = 2;
            int countTry = 0;
            while (true)
            {
                try
                {
                    response = await _httpClient.PostAsJsonAsync("chat/completions", requestBody);
                    response.EnsureSuccessStatusCode();
                    break;
                }
                catch (HttpRequestException)
                {
                    countTry++;
                    if (retries-- <= 0) throw;
                    Console.WriteLine("OpenAI not ready, retrying in 1s...");
                    await Task.Delay(1000);
                }
            }
            if(countTry > 2)
            {
                return "Airy ran out of funds. :money:";
            }
            var result = await response.Content.ReadFromJsonAsync<OpenAIResponse>();
            Console.WriteLine("[OpenAI] Response:");
            Console.WriteLine(result?.Choices[0]?.Message.Content);

            return result?.Choices[0]?.Message.Content ?? string.Empty;
        }

        private class OpenAIResponse
        {
            [JsonPropertyName("choices")]
            public Choice[] Choices { get; set; }

            public class Choice
            {
                [JsonPropertyName("message")]
                public MessageData Message { get; set; }
            }

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
