using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using ArticleAgent.Plugins;

namespace ArticleAgent.Configurations
{
    public class KernelProvider
    {
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatCompletionService;

        public KernelProvider(IConfiguration configuration)
        {
            var geminiApiKey = configuration.GetValue<string>("Gemini:ApiKey")
                ?? throw new InvalidOperationException("Gemini API Key not configured!");

            var geminiModel = configuration.GetValue("Gemini:Model", "gemini-2.5-pro");

            _kernel = BuildKernel(geminiModel, geminiApiKey);

            _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
        }

        public Kernel Kernel => _kernel;

        public IChatCompletionService ChatCompletionService => _chatCompletionService;

        private static Kernel BuildKernel(string model, string apiKey)
        {


            var kernelBuilder = Kernel.CreateBuilder()
                .AddGoogleAIGeminiChatCompletion(model, apiKey);
            //kernelBuilder.Services.AddSingleton(loggerFactory); // 🔌 Inject logger
            kernelBuilder.Services.AddLogging(logs => logs.AddConsole().SetMinimumLevel(LogLevel.Trace));


            return kernelBuilder.Build();
        }


        public void RegisterPlugins(IServiceProvider sp)
        {
            _kernel.Plugins.AddFromType<ArticlePlugin>("ArticlePlugin", sp);
        }


    }
}
