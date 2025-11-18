using WeatherApp.Web;
using WeatherApp.Web.Components;
using WeatherApp.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// ADD THIS - Enable Interactive Server Components
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var apiUrl = Environment.GetEnvironmentVariable("WEATHERAPP_API_URL")
    ?? builder.Configuration["services:weatherapp-api:https:0"]
    ?? builder.Configuration["services:weatherapp-api:http:0"]
    ?? "https://localhost:7581";

builder.Services.AddHttpClient<IWeatherApiClient, WeatherApiClient>(client =>
{
    client.BaseAddress = new Uri(apiUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapDefaultEndpoints();

// ADD THIS - Enable Interactive Server Render Mode
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();