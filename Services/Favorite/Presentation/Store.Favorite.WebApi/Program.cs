using Microsoft.EntityFrameworkCore;
using Store.Favorite.Infrastructure.Context;

using Store.Favorite.Application.Interfaces;
using Store.Favorite.Infrastructure.Repositories;
using MediatR;
using Store.Favorite.Application;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var identityServerUrl = builder.Configuration["IdentityServerURL"]!;
IList<SecurityKey> signingKeys = new List<SecurityKey>();

try
{
    var sslBypassHandler = new HttpClientHandler();
#if DEBUG
    sslBypassHandler.ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true;
#endif
    using var httpClient = new HttpClient(sslBypassHandler);

    // Wait for IdentityServer to be available (Multiple Startup Projects race condition fix)
    int retries = 10;
    while (retries > 0)
    {
        try
        {
            var discoveryJson = await httpClient.GetStringAsync($"{identityServerUrl}/.well-known/openid-configuration");
            var discoveryDoc = System.Text.Json.JsonDocument.Parse(discoveryJson);
            var jwksUri = discoveryDoc.RootElement.GetProperty("jwks_uri").GetString()!;

            var jwksJson = await httpClient.GetStringAsync(jwksUri);
            var jwks = new Microsoft.IdentityModel.Tokens.JsonWebKeySet(jwksJson);
            signingKeys = jwks.GetSigningKeys();
            
            Console.WriteLine($"[AUTH] JWKS başarıyla alındı. Anahtar sayısı: {signingKeys.Count}");
            break; // Başarılı olursa döngüden çık
        }
        catch
        {
            retries--;
            if (retries == 0) 
            {
                Console.WriteLine("[AUTH] IdentityServer'a bağlanılamadı. API yetkilendirme olmadan başlatılıyor.");
                break;
            }
            Console.WriteLine($"[AUTH] IdentityServer'a bağlanılamadı. Tekrar deneniyor... Kalan deneme: {retries}");
            await Task.Delay(2000); // 2 saniye bekle
        }
    }
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
        ValidAudience = "ResourceFavorite",
        ValidateIssuer = true,
        ValidIssuer = identityServerUrl,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKeys = signingKeys
    };
});

// Add services to the container.
builder.Services.AddDbContext<StoreFavoriteContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("FavoriteDatabase"));
});

builder.Services.AddScoped<IFavoriteItemRepository, FavoriteItemRepository>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Store.Favorite.Application.Features.Mediator.Commands.FavoriteItemCommands.AddFavoriteCommand).Assembly));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Store.Favorite.WebApi", Version = "v1" });

    // JWT Auth desteği
    var securityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter JWT Bearer token **_only_**",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer", // must be lower case
        BearerFormat = "JWT",
        Reference = new Microsoft.OpenApi.Models.OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {securityScheme, new string[] { }}
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<StoreFavoriteContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
