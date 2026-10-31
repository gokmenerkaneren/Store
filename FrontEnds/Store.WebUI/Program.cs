using Store.Shared.Middlewares;
using Refit;
using Store.WebUI.Services.Interfaces;
using MediatR;
using Microsoft.Extensions.Options;
using Store.WebUI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configuration bind
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opt =>
    {
        opt.LoginPath = "/Uyelik/Index";
        opt.LogoutPath = "/Uyelik/Logout";
        opt.AccessDeniedPath = "/Home/AccessDenied";
        opt.Cookie.HttpOnly = true;
        opt.Cookie.SameSite = SameSiteMode.Strict;
        opt.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        opt.Cookie.Name = "StoreWebUIAuth";
        opt.ExpireTimeSpan = TimeSpan.FromDays(30);
        opt.SlidingExpiration = true;

        opt.Events = new CookieAuthenticationEvents
        {
            OnValidatePrincipal = context =>
            {
                // HttpContext üzerinden DEĞİL, context.Properties üzerinden token okunur (Döngüyü kırar)
                var token = context.Properties.GetTokenValue("access_token");

                if (string.IsNullOrEmpty(token))
                {
                    context.RejectPrincipal();
                    return Task.CompletedTask;
                }

                var handler = new JwtSecurityTokenHandler();
                if (handler.CanReadToken(token))
                {
                    var jwt = handler.ReadJwtToken(token);
                    if (jwt.ValidTo <= DateTime.UtcNow)
                    {
                        context.RejectPrincipal();
                        return Task.CompletedTask;
                    }
                }
                else
                {
                    context.RejectPrincipal();
                    return Task.CompletedTask;
                }

                return Task.CompletedTask;
            }
        };
    });

// SSL doğrulamasını bypass eden named HttpClient (geliştirme ortamı için)
builder.Services.AddHttpClient("ApiClient", (sp, client) =>
{
    var apiSettings = sp.GetRequiredService<IOptions<ApiSettings>>().Value;
    if (!string.IsNullOrEmpty(apiSettings.LocationBaseUrl))
    {
        client.BaseAddress = new Uri(apiSettings.LocationBaseUrl);
    }
})
.ConfigurePrimaryHttpMessageHandler(() =>
    new HttpClientHandler
    {
#if DEBUG
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
#endif
    });

try
{
    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
}
catch (System.Reflection.ReflectionTypeLoadException ex)
{
    Console.WriteLine("=== ReflectionTypeLoadException Detected ===");
    Console.WriteLine($"Exception Message: {ex.Message}");
    Console.WriteLine("\n=== Failed Types ===");
    foreach (var t in ex.Types.Where(x => x != null))
    {
        Console.WriteLine($"Type: {t?.FullName}");
    }

    Console.WriteLine("\n=== Loader Exceptions ===");
    foreach (var e in ex.LoaderExceptions)
    {
        Console.WriteLine($"Loader Exception: {e?.Message}");
        if (e?.InnerException != null)
        {
            Console.WriteLine($"  Inner Exception: {e.InnerException.Message}");
        }
    }
    throw;
}

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<Store.WebUI.Handlers.BearerTokenHandler>();

Func<HttpMessageHandler> getBypassHandler = () =>
{
#if DEBUG
    return new HttpClientHandler { ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true };
#else
    return new HttpClientHandler();
#endif
};

// Refit Clients
builder.Services.AddRefitClient<IBrandService>().ConfigureHttpClient((sp, c) => c.BaseAddress = new Uri(sp.GetRequiredService<IOptions<ApiSettings>>().Value.CatalogBaseUrl)).AddHttpMessageHandler<Store.WebUI.Handlers.BearerTokenHandler>().ConfigurePrimaryHttpMessageHandler(getBypassHandler);
builder.Services.AddRefitClient<ICategoryService>().ConfigureHttpClient((sp, c) => c.BaseAddress = new Uri(sp.GetRequiredService<IOptions<ApiSettings>>().Value.CatalogBaseUrl)).AddHttpMessageHandler<Store.WebUI.Handlers.BearerTokenHandler>().ConfigurePrimaryHttpMessageHandler(getBypassHandler);
builder.Services.AddRefitClient<IProductService>().ConfigureHttpClient((sp, c) => c.BaseAddress = new Uri(sp.GetRequiredService<IOptions<ApiSettings>>().Value.CatalogBaseUrl)).AddHttpMessageHandler<Store.WebUI.Handlers.BearerTokenHandler>().ConfigurePrimaryHttpMessageHandler(getBypassHandler);
builder.Services.AddRefitClient<IInstagramPostService>().ConfigureHttpClient((sp, c) => c.BaseAddress = new Uri(sp.GetRequiredService<IOptions<ApiSettings>>().Value.CatalogBaseUrl)).AddHttpMessageHandler<Store.WebUI.Handlers.BearerTokenHandler>().ConfigurePrimaryHttpMessageHandler(getBypassHandler);
builder.Services.AddRefitClient<ICatalogApiService>().ConfigureHttpClient((sp, c) => c.BaseAddress = new Uri(sp.GetRequiredService<IOptions<ApiSettings>>().Value.CatalogBaseUrl)).AddHttpMessageHandler<Store.WebUI.Handlers.BearerTokenHandler>().ConfigurePrimaryHttpMessageHandler(getBypassHandler);
builder.Services.AddRefitClient<IColorService>().ConfigureHttpClient((sp, c) => c.BaseAddress = new Uri(sp.GetRequiredService<IOptions<ApiSettings>>().Value.CatalogBaseUrl)).AddHttpMessageHandler<Store.WebUI.Handlers.BearerTokenHandler>().ConfigurePrimaryHttpMessageHandler(getBypassHandler);
builder.Services.AddRefitClient<ISizeService>().ConfigureHttpClient((sp, c) => c.BaseAddress = new Uri(sp.GetRequiredService<IOptions<ApiSettings>>().Value.CatalogBaseUrl)).AddHttpMessageHandler<Store.WebUI.Handlers.BearerTokenHandler>().ConfigurePrimaryHttpMessageHandler(getBypassHandler);
builder.Services.AddRefitClient<IDefaultSlider1Service>().ConfigureHttpClient((sp, c) => c.BaseAddress = new Uri(sp.GetRequiredService<IOptions<ApiSettings>>().Value.CatalogBaseUrl)).AddHttpMessageHandler<Store.WebUI.Handlers.BearerTokenHandler>().ConfigurePrimaryHttpMessageHandler(getBypassHandler);
builder.Services.AddRefitClient<IDefaultTrendService>().ConfigureHttpClient((sp, c) => c.BaseAddress = new Uri(sp.GetRequiredService<IOptions<ApiSettings>>().Value.CatalogBaseUrl)).AddHttpMessageHandler<Store.WebUI.Handlers.BearerTokenHandler>().ConfigurePrimaryHttpMessageHandler(getBypassHandler);
builder.Services.AddRefitClient<IBestSellerApiService>().ConfigureHttpClient((sp, c) => c.BaseAddress = new Uri(sp.GetRequiredService<IOptions<ApiSettings>>().Value.CatalogBaseUrl)).AddHttpMessageHandler<Store.WebUI.Handlers.BearerTokenHandler>().ConfigurePrimaryHttpMessageHandler(getBypassHandler);
builder.Services.AddRefitClient<INewProductApiService>().ConfigureHttpClient((sp, c) => c.BaseAddress = new Uri(sp.GetRequiredService<IOptions<ApiSettings>>().Value.CatalogBaseUrl)).AddHttpMessageHandler<Store.WebUI.Handlers.BearerTokenHandler>().ConfigurePrimaryHttpMessageHandler(getBypassHandler);
builder.Services.AddRefitClient<IFavoriteApiService>().ConfigureHttpClient((sp, c) => c.BaseAddress = new Uri(sp.GetRequiredService<IOptions<ApiSettings>>().Value.FavoriteBaseUrl)).AddHttpMessageHandler<Store.WebUI.Handlers.BearerTokenHandler>().ConfigurePrimaryHttpMessageHandler(getBypassHandler);
builder.Services.AddRefitClient<IBasketApiService>().ConfigureHttpClient((sp, c) => c.BaseAddress = new Uri(sp.GetRequiredService<IOptions<ApiSettings>>().Value.BasketBaseUrl)).AddHttpMessageHandler<Store.WebUI.Handlers.BearerTokenHandler>().ConfigurePrimaryHttpMessageHandler(getBypassHandler);
builder.Services.AddScoped<Store.WebUI.Services.IBasketStorageService, Store.WebUI.Services.BasketStorageService>();
builder.Services.AddSingleton<Store.WebUI.Services.IServiceHealthCheckService, Store.WebUI.Services.ServiceHealthCheckService>();
builder.Services.AddRefitClient<IOrderApiService>().ConfigureHttpClient((sp, c) => c.BaseAddress = new Uri(sp.GetRequiredService<IOptions<ApiSettings>>().Value.OrderBaseUrl)).AddHttpMessageHandler<Store.WebUI.Handlers.BearerTokenHandler>().ConfigurePrimaryHttpMessageHandler(getBypassHandler);

// User and Location (Admin Panel)
builder.Services.AddRefitClient<IIdentityApiService>().ConfigureHttpClient((sp, c) => c.BaseAddress = new Uri(sp.GetRequiredService<IOptions<ApiSettings>>().Value.IdentityServerUrl)).ConfigurePrimaryHttpMessageHandler(getBypassHandler);
builder.Services.AddRefitClient<IUserRegisterApiService>().ConfigureHttpClient((sp, c) => c.BaseAddress = new Uri(sp.GetRequiredService<IOptions<ApiSettings>>().Value.IdentityServerUrl)).ConfigurePrimaryHttpMessageHandler(getBypassHandler);
builder.Services.AddRefitClient<ILocationApiService>().ConfigureHttpClient((sp, c) => c.BaseAddress = new Uri(sp.GetRequiredService<IOptions<ApiSettings>>().Value.LocationBaseUrl)).AddHttpMessageHandler<Store.WebUI.Handlers.BearerTokenHandler>().ConfigurePrimaryHttpMessageHandler(getBypassHandler);
builder.Services.AddRefitClient<IRentalService>().ConfigureHttpClient((sp, c) => c.BaseAddress = new Uri(sp.GetRequiredService<IOptions<ApiSettings>>().Value.RentalBaseUrl)).AddHttpMessageHandler<Store.WebUI.Handlers.BearerTokenHandler>().ConfigurePrimaryHttpMessageHandler(getBypassHandler);

var app = builder.Build();

// Global Exception Handler
app.UseGlobalExceptionHandling();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// 1. AREA ROTASI
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// 2. VARSAYILAN ROTA
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Uygulama Başlarken Servis Bağlantı Kontrolü ve Loglama
app.Lifetime.ApplicationStarted.Register(() =>
{
    _ = Task.Run(async () =>
    {
        try
        {
            var healthCheckService = app.Services.GetRequiredService<Store.WebUI.Services.IServiceHealthCheckService>();
            await healthCheckService.CheckAndLogAsync("Uygulama Başlangıcı (Application Startup)");
        }
        catch (Exception ex)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Uygulama başlangıcı servis kontrolü sırasında hata oluştu.");
        }
    });
});

app.Run();