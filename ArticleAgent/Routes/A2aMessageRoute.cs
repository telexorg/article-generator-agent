using ArticleAgent.DTOs;
using ArticleAgent.Helper;
using ArticleAgent.Services.Implementation;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ArticleAgent.Routes
{
    public class A2aMessageRoute
    {

        public static async Task<IResult> HandleA2aTaskRequest(
            [FromBody] A2aTaskRequest request,
            [FromServices] ArticleService articleService,
            [FromServices] TaskContextAccessor _taskContextAccessor,
            [FromServices] ILogger<A2aMessageRoute> _logger)
        {
            if (request == null)
                return Results.BadRequest("Invalid request");

            _logger.LogInformation($"Task processing started for task {request.Id}");

            ValidationHelper.ValidateRequest(request);

            var contextSnapshot = _taskContextAccessor.GetTaskContext();


            Task.Run(() =>
            {
                _logger.LogInformation($"Processing task {request.Id} in background");
                _taskContextAccessor.SetTaskContext(contextSnapshot);
                articleService.HandleUserMessageAsync(request);
            });

            _logger.LogInformation($"Task {request.Id} submitted");
            var response = DataBuilder.ConstructTaskReceivedResponse(request);

            _logger.LogInformation($"Task received response: {JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true })}");

            return Results.Ok(response);
        }

    }
}
