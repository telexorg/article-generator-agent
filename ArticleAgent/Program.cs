
using HtmlAgilityPack;
using System.Text.Json;
using System.Text;
using ArticleAgent.Services.Implementation;
using ArticleAgent.Helper;
using Microsoft.AspNetCore.Mvc;
using ArticleAgent.Common;
using ArticleAgent.DTOs;
using ArticleAgent.Middlewares;
using ArticleAgent.Configurations;
using Microsoft.Extensions.Caching.Memory;
using ArticleAgent.Plugins;
using Microsoft.SemanticKernel;
using ArticleAgent.Data;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

builder.Services.Configure<TelexApiSettings>(builder.Configuration.GetSection("TelexApiSettings"));

builder.Services.AddSingleton<KernelProvider>();
builder.Services.AddSingleton<ArticlePlugin>();
builder.Services.AddScoped<ArticleService>();
//builder.Services.AddScoped<WebScraper>();
builder.Services.AddScoped<GeminiService>();
builder.Services.AddScoped<SummarizerService>();
builder.Services.AddScoped<TaskContextAccessor>();
builder.Services.AddScoped<DataContext>();
builder.Services.AddScoped<HttpHelper>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddHttpClient();



// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddMemoryCache();
builder.Services.AddScoped<IMemoryCache, MemoryCache>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin", policy =>
    {
        policy.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var kernelProvider = scope.ServiceProvider.GetRequiredService<KernelProvider>();
    kernelProvider.RegisterPlugins(scope.ServiceProvider);
}

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); // Link to the JSON spec
        c.RoutePrefix = ""; // Swagger UI served at /docs
    });
    app.MapOpenApi();
}

app.UseCors("AllowAnyOrigin");

app.UseMiddleware<ExceptionHandler>();
app.UseMiddleware<RequestLogger>();
app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapPost("/api", HandleA2aTaskRequest)
   .WithName("GenerateArticle") // Optional: Swagger operationId
   .WithOpenApi();

async Task<IResult> HandleA2aTaskRequest(
    [FromBody] A2aTaskRequest request,
    [FromServices] ArticleService articleService,
    [FromServices] TaskContextAccessor _taskContextAccessor,
    [FromServices] ILogger<Program> _logger)
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

    _logger.LogInformation($"Task received response: {JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true})}");

    return Results.Json(response);
}


app.MapGet("/api/.well-known/agent.json", async () =>
{
    //var body = await JsonSerializer.DeserializeAsync<RequestData>(request.Body);
    var response = AgentCard.Get();
    if (response == null)
    {
        return Results.BadRequest("Couldn't retrieve agent card");

    }
    return Results.Json(response);
});

app.Run();


record RequestData(string url);
