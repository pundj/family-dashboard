using FamilyDashboard.Api;
using FamilyDashboard.Api.Data;
using FamilyDashboard.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient("OpenMeteo", client => client.BaseAddress = new Uri("https://api.open-meteo.com/v1/"));
builder.Services.AddHttpClient("Nws", client =>
{
    client.BaseAddress = new Uri("https://api.weather.gov/");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/geo+json"));
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.DefaultRequestHeaders.UserAgent.ParseAdd("FamilyDashboard/1.0 (+https://github.com/pundj/family-dashboard)");
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=App_Data/familydashboard.db";

var appDataDirectory = Path.Combine(builder.Environment.ContentRootPath, "App_Data");
Directory.CreateDirectory(appDataDirectory);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services
    .AddIdentityCore<IdentityUser>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 8;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager();

builder.Services
    .AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddIdentityCookies();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.SlidingExpiration = true;
});

builder.Services.AddAuthorization();

builder.Services.Configure<SmartThingsOptions>(builder.Configuration.GetSection("SmartThings"));
builder.Services.AddScoped<ISmartThingsCredentialStore, SmartThingsCredentialStore>();
builder.Services.AddScoped<ISmartThingsProxyService, SmartThingsProxyService>();
builder.Services.AddScoped<IWeatherProxyService, WeatherProxyService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();

    dbContext.Database.ExecuteSqlRaw(@"
        CREATE TABLE IF NOT EXISTS CalendarPreferences (
            PreferenceKey TEXT NOT NULL CONSTRAINT PK_CalendarPreferences PRIMARY KEY,
            PreferencesJson TEXT NOT NULL,
            UpdatedUtc TEXT NOT NULL
        );");
}

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapDefaultEndpoints();
app.MapFallbackToFile("index.html");

app.Run();
