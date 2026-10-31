using MassTransit;
using Store.Payment.Consumers;
using Store.Payment.Services;
using Store.Shared.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IPaymentGateway, MockPaymentGateway>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<StockReservedPaymentConsumer>();

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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseGlobalExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
