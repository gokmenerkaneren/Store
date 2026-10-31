using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Order.Application.Services
{
    public static class ServiceRegistration
    {
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceRegistration).Assembly));
            // Yukarıdaki yapı ilgili kütüphanemizin registrasyonunu sağlamış olacak.
            // MediatR kütüphanesi, CQRS desenini uygularken kullanılan bir kütüphanedir ve bu yapı sayesinde
            // MediatR'ın ilgili assembly'deki tüm handler'ları otomatik olarak bulup kaydetmesini sağlıyoruz.
        }
    }
}
