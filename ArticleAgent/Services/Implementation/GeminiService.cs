using ArticleAgent.Configurations;
using ArticleAgent.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using System;
using System.Text.Json;

namespace ArticleAgent.Services.Implementation
{
    public class GeminiService
    {
        private readonly string _apiKey;
        private readonly KernelProvider _kernelProvider;
        private readonly ILogger<GeminiService> _logger;
        public GeminiService(IConfiguration config, KernelProvider kernelProvider, ILogger<GeminiService> logger)
        {

            _apiKey = config.GetValue<string>("Gemini:ApiKey") ?? throw new ArgumentNullException("Gemini API Key is not configured.");
            _kernelProvider = kernelProvider;
            _logger = logger;
        }

        public async Task<string> GenerateAsync(string systemPrompt, TaskContext taskRequest = null, IEnumerable<ChatMessageContent> messages = null, string userMessage = null)
        {
            try
            {
                var kernel = _kernelProvider.Kernel;
                var chatService = _kernelProvider.ChatCompletionService;

                // Save user message

                var history = new ChatHistory();

                // Add system message to guide the assistant
                history.AddSystemMessage(systemPrompt);

                if (messages != null)
                {
                    history.AddRange(messages);
                }

                if (userMessage != null)
                {
                    history.AddUserMessage(userMessage);
                }
                var executionSettings = new GeminiPromptExecutionSettings
                {
                    ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions,

                    Temperature = 0.7,               // Controls randomness; lower is more deterministic
                    TopP = 0.9,                      // Nucleus sampling; focuses on top cumulative probability tokens
                    TopK = 40,                       // Limits sampling to top-k probable tokens
                };

                var result = await chatService.GetChatMessageContentAsync(
                    history,
                    executionSettings: executionSettings,
                    kernel: kernel
                );                

                return result.Content ?? "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An Error occured during AI Chat with message: {ex.Message}");
                return "Sorry, something went wrong.";
            }
        }


        public async Task<string> GenerateContentAsync(string contentToSummarize)
        {
            var count = contentToSummarize.Length;

            // 2. Call Gemini API
            var geminiRequest = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = contentToSummarize }
                        }
                    }
                }
            };

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-goog-api-key", _apiKey);

            var response = await client.PostAsJsonAsync(
                "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent",
                geminiRequest
            );

            if (!response.IsSuccessStatusCode)
                return null;

            var resultJson = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(resultJson);

            var summary = jsonDoc
                .RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return summary;
        }
    }
}
