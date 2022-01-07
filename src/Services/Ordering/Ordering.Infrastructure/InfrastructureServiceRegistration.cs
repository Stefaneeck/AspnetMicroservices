using Microsoft.Extensions.DependencyInjection;
using Ordering.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Ordering.Application.Contracts.Persistence;
using Ordering.Infrastructure.Repositories;
using Ordering.Application.Models;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Infrastructure.Mail;

namespace Ordering.Infrastructure
{
    //extension methods must be in static class
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            //ef core
            services.AddDbContext<OrderContext>(options => options.UseSqlServer(configuration.GetConnectionString("OrderingConnectionString")));

            //these are defined in the application layer, but the registration is handled in the infrastructure layer
            //typeof for the generic class DI registration
            //addscoped: DI will use the object as per request lifecycle
            services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));
            services.AddScoped<IOrderRepository, OrderRepository>();

            services.Configure<EmailSettings>(c => configuration.GetSection("EmailSettings"));
            //transient: we dont need to create a request, when the application starts for the first time it will create the emailservice and will be using it later
            services.AddTransient<IEmailService, EmailService>();

            return services;
        }

    }
}
