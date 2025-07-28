using ArticleAgent.Helper;

namespace ArticleAgent.Services.Implementation
{
    public class SummarizerService
    {
        //private readonly WebScraper _webScraper;
        //private readonly GeminiService _geminiService;
        private const int MaxTokens = 1000; // Gemini limit per summary chunk
        private const int TokensPerWord = 4; // Rough estimate (1 token ≈ 0.75 words)


        //public ContentSummarizerService(WebScraper webScraper, GeminiService geminiService) 
        //{
        //    _webScraper = webScraper;
        //    _geminiService = geminiService;
        //}




        public static async Task<string> SummarizeContentAsync(string rawContent)
        {
            
            var maxWords = MaxTokens * TokensPerWord; // e.g., 1000 * 4 = 4000
            var stride = (int)(maxWords * 0.2);        // 80% overlap = move forward 20%

            var words = rawContent.Split(new[] { ' ', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> chunkSummaries = new();

            for (int i = 0; i < words.Length; i += stride)
            {
                var chunkWords = words.Skip(i).Take(maxWords).ToArray();
                if (chunkWords.Length == 0) continue;

                var chunkText = string.Join(" ", chunkWords);
                var summary = await GeminiService.GenerateContentAsync(PromptTemplate.SummarizeContentPrompt(chunkText));
                chunkSummaries.Add(summary);

                // Early break if we only need one chunk
                if (chunkWords.Length < maxWords) break;
            }

            // 2. Return directly if only one chunk
            if (chunkSummaries.Count == 1)
                return chunkSummaries[0];

            // 3. Combine and polish if multiple
            var combined = string.Join("\n\n", chunkSummaries);
            var polishedSummary = await GeminiService.GenerateContentAsync(combined);

            return polishedSummary;
        }


    }
}
