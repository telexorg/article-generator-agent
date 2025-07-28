namespace ArticleAgent.Helper
{
    public class PromptTemplate
    {
        public static string SummarizeContentPrompt(string contentToSummarize) =>
            $"""
                You are summarizing part of a longer article. Write a brief, standalone summary that flows naturally and preserves important context.

                Here is the content to summarize:
                {contentToSummarize}
                """;
        
        
        public static string GenerateArticlePrompt(string contentToSummarize) =>
            $"""
                Please use the summarized web content provided to you to write a comprehensive and well-structured article based on the information you find.
            
                Synthesize the key points, main ideas, and important data from these sources into a coherent narrative.
                The article should have a clear introduction, body, and conclusion. 
                
                Format the article in Markdown using proper blank lines between sections.(e.g., use '#' for titles, '##' for headings, '*' for bullet points).
            
                Do not use "\\n" or" \\n\\n" — use real paragraph spacing

                Here is the web content: 
                {contentToSummarize}
            """;

    }
}
