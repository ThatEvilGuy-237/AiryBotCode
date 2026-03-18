using AiryBotCode.Application.Interfaces.Service;
using System.Text.Json;
using System.Net.Http.Json;

namespace AiryBotCode.Application.Services
{
    public class InterpreterService : IInterpreterService
    {
        private readonly HttpClient _httpClient;

        public InterpreterService(string sandboxUrl)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(sandboxUrl)
            };
        }

        public async Task<string> ExecuteCommandAsync(string command)
        {
            var request = new { command };
            var response = await _httpClient.PostAsJsonAsync("execute", request);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<CommandResult>();
            return result?.Output ?? result?.Error ?? "No output";
        }

        public async Task<string> ReadFileAsync(string path)
        {
            var response = await _httpClient.GetAsync($"read?path={Uri.EscapeDataString(path)}");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<FileResult>();
            return result?.Content ?? "File not found or empty";
        }

        public async Task<string> WriteFileAsync(string path, string content)
        {
            var request = new { path, content };
            var response = await _httpClient.PostAsJsonAsync("write", request);
            response.EnsureSuccessStatusCode();
            return "File written successfully";
        }

        private class CommandResult
        {
            public string Output { get; set; }
            public string Error { get; set; }
        }

        private class FileResult
        {
            public string Content { get; set; }
        }
    }
}
