using System.Text.Json;

namespace ArticleAgent.Common
{
    public class AgentCard
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Url { get; set; } = "";
        public string Version { get; set; } = "";
        public string IconUrl { get; set; } = "";
        public string DocumentationUrl { get; set; } = "";
        public Capability Capabilities { get; set; }
        public string[] DefaultInputModes { get; set; }
        public string[] DefaultOutputModes { get; set; }
        public Skill[] Skills { get; set; }
        public AgentProvider Provider { get; set; }
        public List<Dictionary<string, List<string>>> Security { get; set; }
        public Dictionary<string, ISecurityScheme> SecuritySchemes { get; set; }
        public bool SuppostsAuthenticatedExtendedCard { get; set; }

        public static string Get()
        {
            var agentA2A = new AgentCard
            {
                Name = "Article Agent",
                Description = "Article Agent uses AI to extract, summarize, and generate structured articles from one or more URLs with minimal input.",
                Url = "https://telex-article-agent.onrender.com/",
                Version = "1.0.0",
                IconUrl = "https://res.cloudinary.com/dlu45noef/image/upload/v1742882213/article-agent-icon.jpg",
                DocumentationUrl = "https://telex-article-agent-docs.onrender.com",
                Capabilities = new Capability
                {
                    Streaming = false,
                    PushNotifications = true
                },
                DefaultInputModes = new[] { "application/json" },
                DefaultOutputModes = new[] { "application/json" },
                Provider = new AgentProvider
                {
                    Organization = "AI Article Services Org",
                    Url = "https://telex-article-agent.onrender.com"
                },
                Skills = new[]
                {
                    new Skill
                    {
                        Id = "generate-article",
                        Name = "Generate Article from URLs",
                        Description = "Extracts content from URLs and generates high-quality, well-structured articles using AI.",
                        Tags = [
                            "article generation",
                            "content summarization",
                            "AI writing",
                            "web scraping"
                        ],
                        Examples = new[]
                        {
                            "Paste one or more URLs to get a summarized and readable article.",
                            "Supports dynamic websites using Puppeteer for deeper scraping.",
                            "Chunks and summarizes large pages to fit model token limits.",
                            "Outputs content in Markdown format for easy publishing.",
                            "Can create structured blog posts, SEO content, or summaries."
                        }
                    }
                }
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return JsonSerializer.Serialize(agentA2A, options);
        }


        public ISecurityScheme HandleSecurityScheme(ISecurityScheme scheme)
        {
            switch (scheme.Type)
            {
                case "apiKey":
                    return scheme as APIKeySecurityScheme;
                    //Console.WriteLine(api.Name);
                    break;
                case "oauth2":
                    return scheme as OAuth2SecurityScheme;
                    //Console.WriteLine(oauth.Flows.Count);
                    break;
                default:
                    return scheme as OpenIdConnectSecurityScheme;
            }
        }

    }

    public class AgentProvider
    {
        public string Organization { get; set; } = "";
        public string Url { get; set; } = "";
    }

    public class Capability
    {
        public bool Streaming { get; set; }
        public bool PushNotifications { get; set; }
        public bool StateTransitionHistory { get; set; }
    }

    public class Skill
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string[] Tags { get; set; }
        public string[] Examples { get; set; }
    }

    public class OpenIdConnectSecurityScheme : ISecurityScheme
    {
        public string Type { get; set; } = "openId";
        public string OpenIdConnectUrl { get; set; }
    }

    public interface ISecurityScheme
    {
        public string Type { get; set; }
    }

    public class APIKeySecurityScheme : ISecurityScheme
    {
        public string Type { get; set; } = "apiKey";
        public string Name { get; set; }        // Name of the header/query/cookie
        public string In { get; set; }          // "query", "header", "cookie"
    }

    public class OAuth2SecurityScheme : ISecurityScheme
    {
        public string Type { get; set; } = "oauth2";
        public Dictionary<string, OAuthFlow> Flows { get; set; }
    }

    public class OAuthFlow
    {
        public string AuthorizationUrl { get; set; }
        public string TokenUrl { get; set; }
        public string RefreshUrl { get; set; }
        public Dictionary<string, string> Scopes { get; set; }
    }

    public class HTTPAuthSecurityScheme : ISecurityScheme
    {
        public string Type { get; set; } = "jwt";
        public string Scheme { get; set; }          // e.g. "basic", "bearer"
        public string BearerFormat { get; set; }    // e.g. "JWT" (optional)
    }
}
