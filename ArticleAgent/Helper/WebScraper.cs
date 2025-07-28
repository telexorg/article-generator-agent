using HtmlAgilityPack;

namespace ArticleAgent.Helper
{
    public class WebScraper
    {
        public async static Task<List<string>> ScrapeContentAsync1(string url)
        {
            //using var httpClient = new HttpClient();
            //var response = await httpClient.GetStringAsync(url);
            
            // 1. Scrape the page
            var html = await new HtmlWeb().LoadFromWebAsync(url);

            var htmlBody = html.DocumentNode.SelectSingleNode("//body");
            htmlBody.SelectNodes("//script|//style")?.ToList().ForEach(script => script.Remove()); // Remove scripts

            return htmlBody.SelectNodes("//p|//h1|//h2|//h3|//h4|//ul|//li|//span")?
                                .Select(p => p.InnerText.Trim())
                                .Where(p => p.Length > 40)
                                .Take(20)
                                .ToList(); // Take top 10 paragraphs
        }

        public static async Task<string> ScrapeContentAsync(string url)
        {
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(url);

            var body = doc.DocumentNode.SelectSingleNode("//body");
            if (body == null) return string.Empty;

            // Remove unwanted elements
            var unwantedNodes = body.SelectNodes("//script|//style|//footer|//nav|//aside|//form|//noscript");
            if (unwantedNodes != null)
            {
                foreach (var node in unwantedNodes)
                    node.Remove();
            }

            // Select relevant content nodes
            var contentNodes = body.SelectNodes("//p|//h1|//h2|//h3|//h4|//ul|//li|//span");
            if (contentNodes == null) return string.Empty;

            // Extract and clean text
            //var result = contentNodes
            //    .Select(node => HtmlEntity.DeEntitize(node.InnerText.Trim()))
            //    .Where(text => !string.IsNullOrWhiteSpace(text) && text.Length > 40)
            //    .Take(20)
            //    .ToList();

            //return result;

            var text = string.Join(" ", contentNodes
               .Select(node => HtmlEntity.DeEntitize(node.InnerText.Trim()))
               .Where(text => !string.IsNullOrWhiteSpace(text) && text.Length > 40));

            return text;
        }

    }
}
