using ArticleAgent.DTOs;
using ArticleAgent.Enums;
using ArticleAgent.Models;
using System.Text.Json;

namespace ArticleAgent.Helper
{
    public class DataBuilder
    {
        public static TaskReceivedResponse ConstructTaskReceivedResponse(A2aTaskRequest request)
        {
            return new TaskReceivedResponse
            {
                Jsonrpc = request.Jsonrpc,
                Id = request.Id,
                Result = new()
                {
                    Id = request.Params.Message.TaskId ?? Guid.NewGuid().ToString(),
                    Status = new()
                    {
                        State = TaskState.Submitted.ToString().ToLower(),
                        Message = new TaskMessage
                        {
                            MessageId = Guid.NewGuid().ToString(),
                            Role = "agent",
                            Kind = "message",
                            Parts = new List<TextPart>
                            {
                                new TextPart
                                {
                                    Kind = "text",
                                    Text = "Task submitted successfully",
                                }
                            }
                        }
                    }

                }
            };
        }

        public static AgentMessageResponse ConstructResponse(A2aTaskRequest request, string response)
        {
            return new AgentMessageResponse
            {
                Jsonrpc = request.Jsonrpc,
                Id = request.Id,
                Result = new TaskMessage()
                {
                    Role = "agent",
                    Kind = "message",
                    MessageId = Guid.NewGuid().ToString(),
                    ContextId = request.Params.Message.ContextId,
                    Parts = new List<TextPart>
                    {
                        new TextPart
                        {
                            Kind = "text",
                            Text = response,
                        }
                    },
                    Metadata = null

                }
            };
        }

        public static AgentTaskResponse ConstructPushNotificationTask(A2aTaskRequest request, string response, string taskId)
        {
            var contextId = request.Params.Message.ContextId;

            return new AgentTaskResponse
            {
                Jsonrpc = request.Jsonrpc,
                Id = request.Id,
                Result = new TaskResult
                {
                    Id = taskId,
                    ContextId = contextId,
                    Status = new Status
                    {
                        State = TaskState.Completed.ToString().ToLower(),
                        Timestamp = DateTime.UtcNow,
                        Message = new TaskMessage
                        {
                            Role = "agent",
                            MessageId = Guid.NewGuid().ToString(),
                            Kind = "message",
                            Parts = new List<TextPart>
                            {
                                new TextPart
                                {
                                    Text = $"Task Completed Successfully",

                                }
                            },
                        }
                    },
                    Artifacts = new List<Artifact>
                    {    new Artifact
                        {
                            ArtifactId = Guid.NewGuid().ToString(),
                            Name = "push_notification_artifact",
                            Parts = new List<TextPart>
                            {
                                new TextPart
                                {
                                    Text = response,

                                }
                            },
                        }
                    },
                }
            };
        }

        public static TaskContext ExtractTaskData(A2aTaskRequest request)
        {
            var message = request?.Params?.Message;
            var config = request?.Params?.Configuration;
            var pushConfig = config?.PushNotificationConfig;
            var metadata = message?.Metadata ?? new Dictionary<string, object>();

            if (message == null || message.Parts == null || !message.Parts.Any())
                throw new ArgumentException("Invalid message structure");

            return new TaskContext
            {
                Message = message.Parts.First().Text ?? string.Empty,
                ContextId = message.ContextId ?? string.Empty,
                TaskId = message.TaskId,
                MessageId = message.MessageId ?? string.Empty,
                OrgId = metadata.TryGetValue("org_id", out var org) ? org?.ToString() ?? string.Empty : string.Empty,
                UserId = metadata.TryGetValue("telex_user_id", out var user) ? user?.ToString() ?? string.Empty : string.Empty,
                ChannelId = metadata.TryGetValue("telex_channel_id", out var channel) ? channel?.ToString() ?? string.Empty : string.Empty,
                //Settings = metadata.TryGetValue("settings", out var settingsObj) && settingsObj != null
                //           ? JsonSerializer.Deserialize<List<Setting>>(settingsObj.ToString()!) ?? new List<Setting>()
                //           : new List<Setting>(),

                AcceptedOutputModes = config?.AcceptedOutputModes ?? new List<string>(),
                CallbackUrl = pushConfig?.Url ?? string.Empty,
                AuthToken = pushConfig?.Authentication?.Credentials ?? string.Empty,
                HistoryLength = config?.HistoryLength ?? 0,
                IsBlocking = config?.Blocking ?? false
            };

        }
    }
}
