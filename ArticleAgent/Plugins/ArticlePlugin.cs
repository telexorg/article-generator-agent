using ArticleAgent.Helper;
using ArticleAgent.Services.Implementation;
using Microsoft.SemanticKernel;
using System;
using System.ComponentModel;

namespace ArticleAgent.Plugins
{
    public class ArticlePlugin
    {

        //private readonly ILogger<BlogAgentFunctions> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public ArticlePlugin(IServiceScopeFactory scopeFactory)
        {
            //_logger = logger;
            _scopeFactory = scopeFactory;
        }

        [KernelFunction("GenerateArticlefromUrl")]
        [Description("Generate a comprehensive article from url.")]
        public async Task<string> GenerateBlogPostAsync(
           [Description("The URLs to scrape")] string url)
        {
            using var scope = _scopeFactory.CreateScope();
            var writerAgent = scope.ServiceProvider.GetRequiredService<ArticleService>();

            try
            {
                var rawContent = await WebScraper.ScrapeContentAsync(url);
                if (string.IsNullOrWhiteSpace(rawContent))
                    return "No content found.";

                var summaryContent = await SummarizerService.SummarizeContentAsync(rawContent);
                if (summaryContent == null)
                {
                    return "Couldn't summarize content";
                }

                var polishedSummary = await GeminiService.GenerateContentAsync(PromptTemplate.GenerateArticlePrompt(summaryContent));
                return polishedSummary;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}, Failed to Write blog post.");
                return "⚠️ Blog generation failed. Please try again.";
            }

        }
    }
}
