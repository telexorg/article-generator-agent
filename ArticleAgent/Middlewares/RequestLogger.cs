﻿using ArticleAgent.DTOs;
using ArticleAgent.Helper;
using ArticleAgent.Models;
using System.Text.Json;
using System.Text;

namespace ArticleAgent.Middlewares
{
    public class RequestLogger
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLogger> _logger;

        public RequestLogger(RequestDelegate next, ILogger<RequestLogger> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, TaskContextAccessor contextAccessor)
        {
            try
            {
                HttpRequest request = context.Request;
                request.EnableBuffering();

                string headersJson = SerializeAsIndentedJson(request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()));
                string queryJson = SerializeAsIndentedJson(request.Query.ToDictionary(q => q.Key, q => q.Value.ToString()));

                string body = await ReadRequestBodyAsync(request);
                string formattedBody = body;

                if (!string.IsNullOrWhiteSpace(body))
                {
                    try
                    {
                        formattedBody = PrettifyJson(body);

                        if (request.Path == "/api/v1/blogger-agent" && request.Method == "POST")
                        {
                            var requestBody = JsonSerializer.Deserialize<A2aTaskRequest>(body, new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });

                            var taskContext = DataBuilder.ExtractTaskData(requestBody);

                            if (taskContext != null)
                            {

                                //context.Items["TaskContext"] = taskContext;
                                contextAccessor.SetTaskContext(taskContext);                        


                            }
                        }
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "❌ JSON Parsing Error: Malformed request body.");
                    }
                }

                _logger.LogInformation("Request logging middleware triggered.");
                _logger.LogInformation("➡️ Incoming Request: {Method} {Path}", request.Method, request.Path);
                _logger.LogInformation("📬 Headers: {Headers}", headersJson);
                _logger.LogInformation("📘 Query Parameters: {Query}", queryJson);
                _logger.LogInformation("📝 Body: {Body}", formattedBody);

                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Exception in RequestLoggingMiddleware");
                throw;
            }
        }

        // Helpers

        private async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            request.Body.Position = 0;
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;
            return body;
        }

        private string SerializeAsIndentedJson(object data)
        {
            return JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        }

        private string PrettifyJson(string json)
        {
            using var jsonDoc = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(jsonDoc, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
