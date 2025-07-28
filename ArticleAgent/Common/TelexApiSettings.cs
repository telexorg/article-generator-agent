namespace ArticleAgent.Common
{
    public class TelexApiSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; }
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
        public string WebSocketUrl { get; set; }

        public const string Header = "X-AGENT-API-KEY";

    }
}
