namespace ArticleAgent.DTOs
{
    public class A2aTaskRequest
    {
        public string Jsonrpc { get; set; }
        public string Id { get; set; }
        public string Method { get; set; }
        public TaskParams Params { get; set; }

       
    }

    public class TaskParams
    {
        public TaskMessage Message { get; set; }
        public TaskConfiguration? Configuration { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
    }

    public class TaskConfiguration
    {
        public List<string> AcceptedOutputModes { get; set; } = new(); // ["text/plain", "image/png", ...]
        public int HistoryLength { get; set; } = 0;
        public bool Blocking { get; set; } = false;
        public PushNotificationConfig PushNotificationConfig { get; set; } = new();
    }

    public class PushNotificationConfig
    {
        public string Url { get; set; } = string.Empty;
        public string? Token { get; set; } // Can be null
        public Authentication Authentication { get; set; } = new();
    }

    public class Authentication
    {
        public List<string> Schemes { get; set; } = new(); // ["TelexApiKey"]
        public string Credentials { get; set; } = string.Empty; // Token or API key value
    }
}
