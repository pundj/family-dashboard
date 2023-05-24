using FamilyDashboard.Blazor;
using FamilyDashboard.Blazor.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddHttpClient();
builder.Services.AddScoped<IRandomJokeService, RandomJokeService>();
builder.Services.AddScoped<IRandomQuoteService, RandomQuoteService>();
builder.Services.AddScoped<IWeatherForecastService, WeatherForecastService>();

await builder.Build().RunAsync();
