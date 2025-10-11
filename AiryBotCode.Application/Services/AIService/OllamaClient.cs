using AiryBotCode.Domain.database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace AiryBotCode.Application.Ollama
{
    public class OllamaClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _modelName;
        private readonly string _baseUrl;
        //private Process _ollamaProcess;

        public OllamaClient(
            string modelName = "hf.co/QuantFactory/L3-Deluxe-Scrambled-Eggs-On-Toast-8B-GGUF:Q3_K_M", //"granite3.1-moe:3b",
            string baseUrl = "http://localhost:11434")
        {
            _modelName = modelName;
            _baseUrl = baseUrl.TrimEnd('/') + "/";
            _httpClient = new HttpClient { BaseAddress = new Uri(_baseUrl) };
        }

        public async Task<string> SendMessageAsync(List<Message> messages)
        {
            var prompt = BuildPrompt(messages);
            Console.WriteLine("[OLLAMA] prompt input:");
            Console.WriteLine(prompt);
            var requestBody = new
            {
                model = _modelName,
                prompt = prompt,
                stream = false
            };

            HttpResponseMessage response;

            int retries = 5;
            while (true)
            {
                try
                {
                    response = await _httpClient.PostAsJsonAsync("api/generate", requestBody);
                    response.EnsureSuccessStatusCode();
                    break;
                }
                catch (HttpRequestException)
                {
                    if (retries-- <= 0) throw;
                    Console.WriteLine("Ollama not ready, retrying in 1s...");
                    await Task.Delay(1000);
                }
            }

            var result = await response.Content.ReadFromJsonAsync<OllamaResponse>();
            Console.WriteLine("[OLLAMA] Response:");
            Console.WriteLine(result);
            return result?.Response ?? string.Empty;
        }

        private string BuildPrompt(List<Message> messages)
        {
            var prompt = "";
            foreach (var msg in messages)
            {
                switch (msg.User.Role)
                {
                    case ChatRole.System:
                        prompt += $"[System]: {msg.Context}\n";
                        break;
                    case ChatRole.User:
                        prompt += $"[User {msg.User.UserName}]: {msg.Context}\n";
                        break;
                    case ChatRole.Assistant:
                        prompt += $"[Assistant]: {msg.Context}\n";
                        break;
                }
            }
            Console.WriteLine("Prompt to Ollama:");
            Console.WriteLine(prompt);
            return prompt.Trim();
        }

        private class OllamaResponse
        {
            [JsonPropertyName("response")]
            public string Response { get; set; }

            [JsonPropertyName("done")]
            public bool Done { get; set; }
        }
    }
}
