using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Store.Shared.Behaviors;
using System.Reflection;
using MediatR;

namespace Store.Location.Application.Services
{
    public static class ServiceRegistration
    {
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            var assembly = Assembly.GetExecutingAssembly();
            
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(assembly);
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            services.AddValidatorsFromAssembly(assembly);

            var handlerTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && (t.Name.EndsWith("CommandHandler") || t.Name.EndsWith("QueryHandler")));

            foreach (var handlerType in handlerTypes)
            {
                services.AddScoped(handlerType);
            }
        }
    }
}
