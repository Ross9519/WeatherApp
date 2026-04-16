using WeatherApp.Components;
using WeatherApp.Cache.Services;
using WeatherApp.Orchestrators.Implementations;
using WeatherApp.Orchestrators.Interfaces;
using WeatherApp.Services.Implementations;
using WeatherApp.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// =====================
// RAZOR COMPONENTS
// =====================
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

// =====================
// HTTP CLIENTS
// =====================
builder.Services.AddHttpClient<IGeocodingService, GeocodingService>()
    .SetHandlerLifetime(TimeSpan.FromMinutes(5));

builder.Services.AddHttpClient<IWeatherService, WeatherService>()
    .SetHandlerLifetime(TimeSpan.FromMinutes(5));

// =====================
// APPLICATION SERVICES
// =====================
builder.Services.AddSingleton<IWeatherCacheService, WeatherCacheService>();
builder.Services.AddScoped<IWeatherOrchestrator, WeatherOrchestrator>();

builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options =>
    {
        options.DetailedErrors = true;
    });

var app = builder.Build();

// =====================
// PIPELINE
// =====================
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();