using Store.Shared.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Store.Discount.Context;
using Store.Discount.Services;

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
        ValidAudience = "ResourceDiscount",
        ValidateIssuer = true,
        ValidIssuer = identityServerUrl,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKeys = signingKeys
    };
});

// Add services to the container.
builder.Services.AddTransient<DapperContext>();
builder.Services.AddTransient<IDiscountService, DiscountService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Global Exception Handler
app.UseGlobalExceptionHandling();

// Configure the HTTP request pipeline.
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

