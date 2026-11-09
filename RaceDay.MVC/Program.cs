using RaceDay.MVC.Services;

var builder = WebApplication.CreateBuilder(args);

// Register MVC.
builder.Services.AddControllersWithViews();

// Register session support for each browser.
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.Name = ".RaceDay.Mvc.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy =
        Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
    options.Cookie.SameSite =
        Microsoft.AspNetCore.Http.SameSiteMode.Lax;
});

// Allow services to access the current browser session.
builder.Services.AddHttpContextAccessor();

// Connect MVC to the RaceDay API.
builder.Services
    .AddHttpClient<RaceDayApiClient>((serviceProvider, client) =>
    {
        var configuration =
            serviceProvider.GetRequiredService<IConfiguration>();

        var baseUrl = configuration["ApiSettings:BaseUrl"];

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new InvalidOperationException(
                "RaceDay API BaseUrl is missing from appsettings.json.");
        }

        client.BaseAddress = new Uri(baseUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
    })
    // We forward the API session cookie ourselves, per browser.
    // This prevents automatic CookieContainer sharing.
    .ConfigurePrimaryHttpMessageHandler(() =>
        new HttpClientHandler
        {
            UseCookies = false
        });

builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Session must run before MVC controllers use it.
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();