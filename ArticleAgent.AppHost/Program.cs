var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.ArticleAgent>("ArticleAgent");

builder.Build().Run();
