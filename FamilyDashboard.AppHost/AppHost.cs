var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.FamilyDashboard_Api>("api")
    .WithExternalHttpEndpoints();

builder.Build().Run();
