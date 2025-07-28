var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.ArticleAgent>("webapplication1");

builder.Build().Run();
