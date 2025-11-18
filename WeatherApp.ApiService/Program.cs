using Microsoft.EntityFrameworkCore;
using WeatherApp.Core.IService;
using WeatherApp.Core.Services;
using WeatherApp.Data.Context;
using WeatherApp.Data.Repositories;
using WeatherApp.Migrations;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add SQL Server DbContext with Aspire
builder.AddSqlServerDbContext<WeatherDbContext>("weatherdb");

builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IWeatherRecordRepository, WeatherRecordRepository>();
builder.Services.AddScoped<IAlertRepository, AlertRepository>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<IWeatherRecordService, WeatherRecordService>();
builder.Services.AddScoped<IAlertService, AlertService>();

// Add OpenWeatherMap client
builder.Services.AddHttpClient<IOpenWeatherClient, OpenWeatherClient>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// Run migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        var context = services.GetRequiredService<WeatherDbContext>();

        logger.LogInformation("Getting connection string...");
        var connectionString = context.Database.GetConnectionString();

        if (!string.IsNullOrEmpty(connectionString))
        {
            logger.LogInformation("Running database migrations...");

            // FluentMigrator will create the database if it doesn't exist
            MigrationRunner.MigrateDatabase(connectionString);

            logger.LogInformation("Database migrations completed successfully");
        }
        else
        {
            logger.LogError("No connection string available");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database");
        // Don't throw - let the app start anyway
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapDefaultEndpoints();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
