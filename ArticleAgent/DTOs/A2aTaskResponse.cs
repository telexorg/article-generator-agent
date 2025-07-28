namespace ArticleAgent.DTOs
{
    public class AgentTaskResponse
    {
        public string Jsonrpc { get; set; }
        public string Id { get; set; }
        public TaskResult Result { get; set; }
        public object? Error { get; set; }  // null here, but could be an error object if present

    }

    public class TaskResult
    {
        public string Id { get; set; }
        public string ContextId { get; set; }
        public Status Status { get; set; }
        public List<Artifact> Artifacts { get; set; }
        public List<TaskMessage> History { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
    }

    public class Status
    {
        public string State { get; set; }
        public DateTime Timestamp { get; set; }
        public TaskMessage Message { get; set; }
    }

    public class Artifact
    {
        public string ArtifactId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<TextPart> Parts { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
        public int? index { get; set; }
        public object? Append { get; set; }
        public object? LastChunk { get; set; }
    }

    public class TaskMessage
    {
        public string? TaskId { get; set; }
        public string MessageId { get; set; }
        public string ContextId { get; set; }
        public string Role { get; set; }
        public string? Kind { get; set; }
        public List<TextPart> Parts { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }

    }

    public class TextPart : ITaskPart
    {
        public string Kind { get; set; } = "text";
        public string Text { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }

    }

    public class FilePart : ITaskPart
    {
        public string Kind { get; set; } = "file";
        public FileContent File { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }

    }

    public class DataPart : ITaskPart
    {
        public string Kind { get; set; } = "data";
        public Dictionary<string, object> Data { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }

    }

    public class FileContent
    {
        public string? Name { get; set; }
        public string MimeType { get; set; }
        public string? Bytes { get; set; }
        public string? Url { get; set; }


    }

    public interface ITaskPart
    {
        string Kind { get; set; }
    }
}
