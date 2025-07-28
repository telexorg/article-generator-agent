using System.Text.Json.Serialization;

namespace ArticleAgent.Common
{
    public class ApiRequest
    {
        public HttpMethod Method { get; set; }
        public string Url { get; set; }
        public object? Body { get; set; }
        public Dictionary<string, string> Headers { get; set; } = new();

    }

    public class TelexChatMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }

    public class TelexChatResponse
    {
        public TelexChatMessage Messages { get; set; } = new();
    }
}
