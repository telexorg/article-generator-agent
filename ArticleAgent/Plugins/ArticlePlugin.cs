using ArticleAgent.Helper;
using ArticleAgent.Services.Implementation;
using Microsoft.SemanticKernel;
using System;
using System.ComponentModel;
using System.Text;

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

        //[KernelFunction("ScrapeWebUrlContent")]
        //[Description("Scrapes the url and return a summary of its content.")]
        //public async Task<string> GenerateBlogPostAsync(1
        //   [Description("The urls to scrape")] List<string> urls)
        //{
        //    using var scope = _scopeFactory.CreateScope();
        //    var writerAgent = scope.ServiceProvider.GetRequiredService<ArticleService>();
        //    var service = scope.ServiceProvider.GetRequiredService<SummarizerService>();

        //    try
        //    {
        //        StringBuilder summaries = new StringBuilder();

        //        foreach (string url in urls)
        //        {
        //            var rawContent = await WebScraper.ScrapeContentAsync(url);
        //            if (string.IsNullOrWhiteSpace(rawContent))
        //                return "No content found.";

        //            var summaryContent = await service.SummarizeContentAsync(rawContent);

        //            summaries.Append(summaryContent);
        //        }

        //        if (summaries == null)
        //        {
        //            return "Couldn't summarize content";
        //        }

        //        return summaries.ToString();
        //        //var polishedSummary = await _geminiService.GenerateContentAsync(PromptTemplate.GenerateArticlePrompt(summaryContent));
        //        //return polishedSummary;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"{ex}, Failed to Write blog post.");
        //        return "⚠️ Blog generation failed. Please try again.";
        //    }

        //}

        [KernelFunction("ScrapeWebUrlContent")]
        public async Task<string> GenerateBlogPostAsync([Description("The URLs to scrape")] List<string> urls)
        {
            using var scope = _scopeFactory.CreateScope();
            var writerAgent = scope.ServiceProvider.GetRequiredService<ArticleService>();
            var service = scope.ServiceProvider.GetRequiredService<SummarizerService>();

            try
            {
                var allSummaries = new List<string>();

                foreach (string url in urls)
                {
                    var rawContent = await WebScraper.ScrapeContentAsync(url);
                    if (string.IsNullOrWhiteSpace(rawContent))
                        continue; // Skip empty URLs instead of failing the whole process

                    var summaryContent = await service.SummarizeContentAsync(rawContent);

                    // Optionally: Add source reference in markdown
                    var sourceReference = $"**Source:** [{url}]({url})\n\n";
                    allSummaries.Add(sourceReference + summaryContent);
                }

                if (allSummaries.Count == 0)
                    return "❌ No valid content found from the provided URLs.";

                // Combine all summaries
                var combinedSummary = string.Join("\n\n---\n\n", allSummaries);

                return combinedSummary;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}, Failed to write blog post.");
                return "⚠️ Blog generation failed. Please try again.";
            }
        }

    }
}
