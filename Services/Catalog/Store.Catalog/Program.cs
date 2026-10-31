using MassTransit;
using Store.Catalog.Consumers;
using Store.Shared.Middlewares;
using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using MediatR;
using Swashbuckle.AspNetCore;
using MongoDB.Driver;
using Serilog;
using Store.Catalog.Extensions;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Hangfire.Dashboard;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using Store.Catalog.Services;
using Store.Catalog.Services.BrandServices;
using Store.Catalog.Services.CacheServices;
using Store.Catalog.Services.CategoryServices;
using Store.Catalog.Services.CurrencyServices;
using Store.Catalog.Services.MeasurementServices;
using Store.Catalog.Services.ProductImageServices;
using Store.Catalog.Services.ProductPriceServices;
using Store.Catalog.Services.ProductServices;
using Store.Catalog.Settings;
using Store.Catalog.Services.PricingServices;
using StackExchange.Redis;


MongoDB.Bson.Serialization.BsonSerializer.RegisterSerializer(new MongoDB.Bson.Serialization.Serializers.GuidSerializer(MongoDB.Bson.GuidRepresentation.Standard));
MongoDB.Bson.Serialization.BsonSerializer.RegisterSerializer(new MongoDB.Bson.Serialization.Serializers.NullableSerializer<Guid>(new MongoDB.Bson.Serialization.Serializers.GuidSerializer(MongoDB.Bson.GuidRepresentation.Standard)));

var builder = WebApplication.CreateBuilder(args);

// Development: SSL doğrulaması bypass
var identityServerUrl = builder.Configuration["IdentityServerURL"]!;
IList<SecurityKey> signingKeys = new List<SecurityKey>();
try
{
    var sslBypassHandler = new HttpClientHandler
    {
#if DEBUG
        ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true
#endif
    };
    using var httpClient = new HttpClient(sslBypassHandler);

    var discoveryJson = await httpClient.GetStringAsync($"{identityServerUrl}/.well-known/openid-configuration");
    var discoveryDoc = System.Text.Json.JsonDocument.Parse(discoveryJson);
    var jwksUri = discoveryDoc.RootElement.GetProperty("jwks_uri").GetString()!;

    var jwksJson = await httpClient.GetStringAsync(jwksUri);
    var jwks = new Microsoft.IdentityModel.Tokens.JsonWebKeySet(jwksJson);
    signingKeys = jwks.GetSigningKeys();
}
catch (Exception ex)
{
    Console.WriteLine($"[AUTH] JWKS alınamadı: {ex.Message}");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = true,
        ValidAudience = "ResourceCatalog",
        ValidateIssuer = true,
        ValidIssuer = identityServerUrl,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKeys = signingKeys
    };
});

// MongoDB Conventions
var conventionPack = new MongoDB.Bson.Serialization.Conventions.ConventionPack
{
    new MongoDB.Bson.Serialization.Conventions.IgnoreExtraElementsConvention(true)
};
MongoDB.Bson.Serialization.Conventions.ConventionRegistry.Register("CatalogStoreConventions", conventionPack, t => true);

builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<Store.Catalog.Services.PriceTypeServices.IPriceTypeService, Store.Catalog.Services.PriceTypeServices.PriceTypeService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IMeasurementService, MeasurementService>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<IProductImageService, ProductImageService>();
builder.Services.AddScoped<IProductPriceService, ProductPriceService>();
builder.Services.AddScoped<Store.Catalog.Services.JobServices.IBulkPriceJobService, Store.Catalog.Services.JobServices.BulkPriceJobService>();
builder.Services.AddScoped<Store.Catalog.Services.DefaultSlider1Services.IDefaultSlider1Service, Store.Catalog.Services.DefaultSlider1Services.DefaultSlider1Service>();
builder.Services.AddScoped<Store.Catalog.Services.DefaultTrendServices.IDefaultTrendService, Store.Catalog.Services.DefaultTrendServices.DefaultTrendService>();
builder.Services.AddScoped<Store.Catalog.Services.BestSellerServices.IBestSellerService, Store.Catalog.Services.BestSellerServices.BestSellerService>();
builder.Services.AddScoped<Store.Catalog.Services.NewProductServices.INewProductService, Store.Catalog.Services.NewProductServices.NewProductService>();
builder.Services.AddScoped<Store.Catalog.Services.InstagramPostServices.IInstagramPostService, Store.Catalog.Services.InstagramPostServices.InstagramPostService>();
// Pricing Service ve Redis
builder.Services.AddScoped<IPricingService, PricingService>();
var redisConn = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
var redisOptions = ConfigurationOptions.Parse(redisConn);
redisOptions.AbortOnConnectFail = false;
// Bağlantı yokken komutlar kuyrukta zaman aşımı beklemesin: her önbellekli okuma ~12 sn gecikiyordu.
redisOptions.BacklogPolicy = BacklogPolicy.FailFast;
builder.Services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(redisOptions));
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.ConfigurationOptions = redisOptions;
    options.InstanceName = CatalogCacheService.InstanceName;
});
builder.Services.AddScoped<ICatalogCacheService, CatalogCacheService>();

// Serilog Configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File("Logs/catalog-log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Configuration
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettings"));

builder.Services.AddSingleton<IDatabaseSettings>(sp =>
{
    return sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
});

// MongoDB Client & Database
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IDatabaseSettings>();
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var settings = sp.GetRequiredService<IDatabaseSettings>();
    return client.GetDatabase(settings.DatabaseName);
});

builder.Services.AddApplicationServices(Assembly.GetExecutingAssembly());

// Hangfire Configuration (MongoDB Storage)
var mongoConnString = builder.Configuration.GetSection("DatabaseSettings:ConnectionString").Value;
var mongoDbName = builder.Configuration.GetSection("DatabaseSettings:DatabaseName").Value;

builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseMongoStorage(mongoConnString, mongoDbName, new MongoStorageOptions
    {
        MigrationOptions = new MongoMigrationOptions
        {
            MigrationStrategy = new MigrateMongoMigrationStrategy(),
            BackupStrategy = new CollectionMongoBackupStrategy()
        },
        Prefix = "hangfire.mongo",
        CheckConnection = true
    })
);
builder.Services.AddHangfireServer();

// MassTransit + RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCreatedConsumer>();

    // Order servisinde aynı adlı consumer var; varsayılan kuyruk adı ortak olsaydı
    // iki servis aynı kuyrukta yarışır ve her mesajı yalnızca biri işlerdi.
    x.AddConsumer<PaymentFailedConsumer>().Endpoint(e => e.Name = "catalog-payment-failed");

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"] ?? "localhost", "/", h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"] ?? "guest");
            h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Store.Catalog.Handlers.GetAllProductsQueryHandler).Assembly));

builder.Services.AddAutoMapper(cfg => { cfg.AddMaps(typeof(Program).Assembly); });
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

UserContextAccessor.Initialize(app.Services.GetRequiredService<IHttpContextAccessor>());

app.UseGlobalExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Hangfire Dashboard Config (Sadece Admin Yetkisi)
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() }
});

app.MapControllers();



try
{
    Log.Information("Catalog Service is starting...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Catalog Service failed to start!");
}
finally
{
    Log.CloseAndFlush();
}

// Local class for Hangfire Authorization
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        // MVP: Admin rolü kontrolü
        return httpContext.User.Identity?.IsAuthenticated == true && httpContext.User.IsInRole("Admin");
    }
}

