namespace ArticleAgent.DTOs
{
    public class TaskReceivedResponse
    {
        public string Jsonrpc { get; set; }
        public string Id { get; set; }
        public TaskReceivedResult Result { get; set; }
    }

    public class TaskReceivedResult
    {
        public string Id { get; set; }
        public Status Status { get; set; }
    }

    public class AgentMessageResponse
    {
        public string Jsonrpc { get; set; }
        public string Id { get; set; }
        public TaskMessage Result { get; set; }
    }
}
