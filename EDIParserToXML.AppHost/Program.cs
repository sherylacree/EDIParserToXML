var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.EDIParserToXML_ApiService>("apiservice");

builder.AddProject<Projects.EDIParserToXML_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
