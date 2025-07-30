
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
using System.Text.Json.Serialization;
using ArticleAgent.Routes;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); 
        c.RoutePrefix = ""; 
    });
    app.MapOpenApi();
}

app.UseCors("AllowAnyOrigin");

app.UseMiddleware<ExceptionHandler>();
app.UseMiddleware<RequestLogger>();
app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapPost("/api", A2aMessageRoute.HandleA2aTaskRequest)
   .WithName("GenerateArticle") // Optional: Swagger operationId
   .WithOpenApi();


app.MapMethods("/health", new[] { "HEAD" }, () =>
{
    return Results.Ok("Service is alive");
});


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
