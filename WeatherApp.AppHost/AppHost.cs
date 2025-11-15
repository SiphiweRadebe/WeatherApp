var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent);

var weatherDb = sqlServer.AddDatabase("weatherdb");

var weatherApi = builder.AddProject<Projects.WeatherApp_ApiService>("weatherapp-api")
    .WithReference(weatherDb)
    .WithExternalHttpEndpoints();

builder.AddProject<Projects.WeatherApp_Web>("weatherapp-web")
    .WithReference(weatherApi)
    .WithExternalHttpEndpoints();

builder.Build().Run();