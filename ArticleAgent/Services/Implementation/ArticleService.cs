using ArticleAgent.Common;
using ArticleAgent.DTOs;
using ArticleAgent.Helper;
using ArticleAgent.Models;

namespace ArticleAgent.Services.Implementation
{
    public class ArticleService
    {

        private readonly HttpHelper _httpHelper;
        private readonly TaskContextAccessor _contextAccessor;
        private readonly ILogger<ArticleService> _logger;
        private readonly GeminiService _geminiService;

        public ArticleService(TaskContextAccessor contextAccessor, HttpHelper httpHelper, ILogger<ArticleService> logger, GeminiService geminiService)
        {
            _httpHelper = httpHelper;
            _contextAccessor = contextAccessor;
            _logger = logger;
            _geminiService = geminiService;
        }

        public async Task HandleUserMessageAsync(A2aTaskRequest request)
        {

            var taskContext = _contextAccessor.GetTaskContext();
            try
            {

                if (taskContext != null && taskContext.TaskId == null)
                {
                    taskContext.TaskId = Guid.NewGuid().ToString();
                    _contextAccessor.SetTaskContext(taskContext);
                }

                _logger.LogInformation("HandleUserInput: processing user message - {Message}", taskContext?.Message);

                var systemPrompt = PromptTemplate.GenerateSystemPrompt();
                var aiReply = await _geminiService.GenerateAsync(systemPrompt, taskContext, userMessage: taskContext.Message);


                var taskResponse = DataBuilder.ConstructPushNotificationTask(request, aiReply, taskContext.TaskId);

                var isSent = await SendResponseAsync(taskResponse, taskContext);

                if (!isSent)
                {
                    _logger.LogInformation("Response sent successfully");
                }
                else 
                    _logger.LogInformation("Failed to send response");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in HandleUserInput()");
                var taskResponse = DataBuilder.ConstructPushNotificationTask(request, "Sorry, something went wrong.", taskContext.TaskId);

                await SendResponseAsync(taskResponse, taskContext);
            }
        }

        public async Task<bool> SendResponseAsync(AgentTaskResponse taskResponse, TaskContext taskContext)
        {

            if (string.IsNullOrEmpty(taskContext.AuthToken))
            {
                throw new Exception("Auth key is required");
            }

            var apiRequest = new ApiRequest()
            {
                Url = taskContext.CallbackUrl,
                Body = taskResponse,
                Method = HttpMethod.Post,
                Headers = new Dictionary<string, string>()
                {
                    {"X-TELEX-API-KEY", taskContext.AuthToken }
                }
            };

            var telexResponse = await _httpHelper.SendRequestAsync(apiRequest);

            if ((int)telexResponse.StatusCode != StatusCodes.Status202Accepted || !telexResponse.IsSuccessStatusCode)
            {
                _logger.LogInformation("Failed to send response to telex");
                return false;
            }

            string responseContent = await telexResponse.Content.ReadAsStringAsync();

            _logger.LogInformation($"Response successfully sent to telex: {responseContent}");

            return true;
        }

    }
}
