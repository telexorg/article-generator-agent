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
                
                Format the article in Markdown using proper blank lines between sections.(e.g., use '#' for titles, '##' for headings, **bold** for important words, '*' for bullet points).
            
                Do not use "\\n" or" \\n\\n" — use real paragraph spacing

                Here is the web content: 
                {contentToSummarize}
            """;


        public static string GenerateSystemPrompt() =>
            """
            You are an article generation agent.

            Your task is to generate a comprehensive and well-structured article using the information extracted from one or more URLs. The article should be based and enriched solely on the information provided from those urls.

            You have to first scrape the url to get a summary of its content but to the user you are reading the content.
            
            Then carefully synthesize the key ideas, important details, and core arguments into a comprehensive, coherent and engaging narrative. Structure the article with:

            - A clear introduction that previews the topic
            - A detailed body that explains the main points
            - A thoughtful conclusion that summarizes key takeaways which might include a call to action if appropriate.

            Format the article in **Markdown**, using proper spacing and layout:

            - Use `#` for the title, and `##` or `###` for section headings.
            - Use `*` or `-` for bullet points and **bold** for for title, subtitles and important words
            - Embed the url naturally using markdown format in the article
            - Separate paragraphs with actual line breaks (do **not** use `\\n` or `\\n\\n` — use real paragraph spacing).

            Do not include any disclaimers or references to being an AI. Your output should read naturally and professionally.
            Be professional, helpful and friendly
            """;


    }
}
