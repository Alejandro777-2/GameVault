using GameVault.Data;
using GameVault.Models;
using GameVault.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// TODO: Re-enable RequireConfirmedAccount = true before deploying to production
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddErrorDescriber<SpanishIdentityErrorDescriber>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IGeocodingService, GoogleGeocodingService>();
builder.Services.AddControllersWithViews();

var esEC = new CultureInfo("es-EC");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(esEC);
    options.SupportedCultures = [esEC];
    options.SupportedUICultures = [esEC];
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRequestLocalization();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    await GameVault.Data.DbSeeder.SeedAsync(scope.ServiceProvider);
    await BackfillCoordinatesAsync(scope.ServiceProvider);
}

app.Run();

// Geocodes any user who has a City but no Latitude/Longitude.
// Runs only at startup in Development; the null-check is the idempotency guard.
static async Task BackfillCoordinatesAsync(IServiceProvider services)
{
    var db       = services.GetRequiredService<ApplicationDbContext>();
    var geocoder = services.GetRequiredService<IGeocodingService>();
    var logger   = services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup.Backfill");

    var users = await db.Users
        .Where(u => u.City != null && u.Latitude == null)
        .ToListAsync();

    if (users.Count == 0) return;

    int backfilled = 0;
    foreach (var user in users)
    {
        var (lat, lng) = await geocoder.GeocodeCityAsync(user.City!);
        if (lat.HasValue && lng.HasValue)
        {
            user.Latitude  = lat;
            user.Longitude = lng;
            backfilled++;
        }
    }

    if (backfilled > 0)
        await db.SaveChangesAsync();

    logger.LogInformation(
        "[Backfill] Coordenadas: {Backfilled}/{Total} usuario(s) geocodificados al inicio.",
        backfilled, users.Count);
}
