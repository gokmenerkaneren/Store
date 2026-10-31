using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Store.Location.Persistence.Context;
using Store.Location.Persistence.Repositories;
using Store.Location.Application.Interfaces;
using Store.Location.Application.Services;
using Store.Shared.Middlewares;
using Store.Location.Presentation.WebApi.Services;


var builder = WebApplication.CreateBuilder(args);

// Development: SSL doğrulamasını atlayan HttpClient ile JWKS anahtarlarını direkt fetch et
var identityServerUrl = builder.Configuration["IdentityServerURL"]!;
IList<SecurityKey> signingKeys = new List<SecurityKey>();
try
{
    var sslBypassHandler = new HttpClientHandler();
#if DEBUG
    sslBypassHandler.ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true;
#endif
    using var httpClient = new HttpClient(sslBypassHandler);

    var discoveryJson = await httpClient.GetStringAsync($"{identityServerUrl}/.well-known/openid-configuration");
    var discoveryDoc = System.Text.Json.JsonDocument.Parse(discoveryJson);
    var jwksUri = discoveryDoc.RootElement.GetProperty("jwks_uri").GetString()!;

    var jwksJson = await httpClient.GetStringAsync(jwksUri);
    var jwks = new Microsoft.IdentityModel.Tokens.JsonWebKeySet(jwksJson);
    signingKeys = jwks.GetSigningKeys();

    Console.WriteLine($"[AUTH] JWKS başarıyla alındı. Anahtar sayısı: {signingKeys.Count}");
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
        ValidAudience = "ResourceLocation",
        ValidateIssuer = true,
        ValidIssuer = identityServerUrl,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKeys = signingKeys
    };
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, UserContext>();
builder.Services.AddDbContext<LocationContext>(options =>
{
    var connStr = builder.Configuration.GetConnectionString("PostgreSqlConnection");
    if (!string.IsNullOrEmpty(connStr))
    {
        options.UseNpgsql(connStr, pgOptions =>
        {
            pgOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorCodesToAdd: null);
        });
    }
});

// Add services to the container.
// IRepository'i register ettim.
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
// Aşağıda da statik sınıfımızı çağıracağız.
builder.Services.AddApplicationServices(builder.Configuration);






builder.Services.AddControllers(options =>
{
    var policy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter(policy));
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseGlobalExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
