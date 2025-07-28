using System.Security.Principal;
using System.Text.Json.Serialization;

namespace ArticleAgent.Models
{
    public class Message : IEntity
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("_id")]
        public string? Id { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("organisation_id")]
        public string? OrganizationId { get; set; }

        [JsonPropertyName("tag")]
        public string Tag { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string ContextId { get; set; }
        public string TaskId { get; set; }
        public string Role { get; set; } = null;
        public string Content { get; set; } = null;
    }
}
