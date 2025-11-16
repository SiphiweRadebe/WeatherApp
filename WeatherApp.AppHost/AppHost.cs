var builder = DistributedApplication.CreateBuilder(args);

var password = builder.AddParameter("sql-password", secret: true);

var sqlServer = builder.AddSqlServer("sql", password)
    .WithLifetime(ContainerLifetime.Persistent);

// Add database - Aspire will create it automatically
var weatherDb = sqlServer.AddDatabase("weatherdb");

var weatherApi = builder.AddProject<Projects.WeatherApp_ApiService>("weatherapp-api")
    .WithReference(weatherDb)
    .WaitFor(weatherDb)
    .WithExternalHttpEndpoints();

builder.AddProject<Projects.WeatherApp_Web>("weatherapp-web")
    .WithReference(weatherApi)
    .WaitFor(weatherApi)
    .WithExternalHttpEndpoints();

builder.Build().Run();