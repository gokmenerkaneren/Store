using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Options;
using Store.Basket.LoginServices;
using Store.Basket.Services;
using Store.Basket.Settings;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["IdentityServerUrl"];
    options.Audience = "ResourceBasket";    // Resource Basketi gönderiyorum.
    options.RequireHttpsMetadata = false;   // IdentityServerUrl'yi https olarak çalıştırmadığım için false yaptım. Eğer https olarak çalıştırırsam true yapacağım.
    options.MapInboundClaims = false;       // sub claim'inin xml schema URI'ına ezilmesini engeller
});

builder.Services.AddHttpContextAccessor(); 
// HttpContextAccessor'ı DI container'a ekliyoruz. Bu sayede, uygulamanın herhangi bir yerinde HttpContext'e erişebiliriz.
builder.Services.AddScoped<ILoginService, LoginService>();  
// LoginService'i DI container'a ekliyoruz. Bu sayede, uygulamanın herhangi bir yerinde ILoginService'i kullanabiliriz.
builder.Services.AddScoped<IBasketService, BasketService>();
// BasketService'i DI container'a ekliyoruz. Bu sayede, uygulamanın herhangi bir yerinde IBasketService'i kullanabiliriz.
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("RedisSettings"));
// RedisSettings'i DI container'a ekliyoruz. Bu sayede, uygulamanın herhangi bir yerinde RedisSettings'e erişebiliriz.
builder.Services.AddSingleton<RedisService>(sp=>
{
    // Aşağıda Redisin bağlantı configürasyonunu yapıyoruz. RedisSettings'i DI container'dan alıyoruz.
    var redisSettings = sp.GetRequiredService<IOptions<RedisSettings>>().Value;
    var redis = new RedisService(redisSettings.Host, redisSettings.Port);  // Host ve Port bilgilerini RedisService'e geçiriyoruz. Önce Hosta, sonra porta ulaşacaksın diyorum.
    // Redis configürasyonunun yaptık.
    redis.Connect();    // Redise bağlan
    return redis;       // Redisi geri dönüyoruz.
});


var requireAuthorizePolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
// Burada değişkenin içerisine bir kullanıcının zorunlu olması gerektiğini atamış olduk.
// Bu sayede, uygulamanın herhangi bir yerinde bu policy'i kullanabiliriz. Gelip aşağıda çağıracağım.


builder.Services.AddControllers(opt=>
{
    opt.Filters.Add(new AuthorizeFilter(requireAuthorizePolicy));
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errorMessages = context.ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => !string.IsNullOrEmpty(e.ErrorMessage) ? e.ErrorMessage : e.Exception?.Message ?? "Geçersiz veri")
            .ToList();

        var detail = string.Join("; ", errorMessages);
        Console.WriteLine($"[Store.Basket] Model Validation Error: {detail}");

        return new BadRequestObjectResult(new
        {
            success = false,
            message = $"Doğrulama hatası: {detail}"
        });
    };
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Store.Basket", Version = "v1" });

    var securityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter JWT Bearer token **_only_**",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
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