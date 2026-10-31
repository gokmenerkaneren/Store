using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Store.B2B.Application.Features.CQRS.Handlers.DealerFinancialHandlers;
using Store.B2B.Application.Interfaces;
using Store.B2B.Domain.Entities;
using Store.B2B.Persistence.Context;
using Store.B2B.Persistence.Repositories;
using Store.Shared.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// ──────────────────────────────────────────────────
// Authentication — JWT Bearer (IdentityServer4)
// ──────────────────────────────────────────────────
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["IdentityServerUrl"];
        options.Audience = "ResourceB2B";
        options.RequireHttpsMetadata = false;
        options.MapInboundClaims = false;
    });

// ──────────────────────────────────────────────────
// Database
// ──────────────────────────────────────────────────
builder.Services.AddDbContext<B2BContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), pgOptions =>
    {
        pgOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorCodesToAdd: null);
    });
});

// ──────────────────────────────────────────────────
// Dependency Injection
// ──────────────────────────────────────────────────
builder.Services.AddScoped<IB2BRepository<DealerFinancial>, B2BRepository<DealerFinancial>>();
builder.Services.AddScoped<GetAllDealerFinancialQueryHandler>();
builder.Services.AddScoped<GetDealerFinancialByIdQueryHandler>();
builder.Services.AddScoped<CreateDealerFinancialCommandHandler>();
builder.Services.AddScoped<UpdateDealerFinancialCommandHandler>();
builder.Services.AddScoped<RemoveDealerFinancialCommandHandler>();

// ──────────────────────────────────────────────────
// ASP.NET Core
// ──────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Global Exception Handler
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
