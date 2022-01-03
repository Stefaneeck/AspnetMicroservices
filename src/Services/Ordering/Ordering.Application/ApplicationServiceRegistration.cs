using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            //services used in application layer

            //checks if there is any class that inherits from Profile, and executes the profiles when running the application
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            //checks if there is any class that inherits from the AbstractValidator
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            //checks if there is any class that inherits from IRequest or IRequestHandler
            services.AddMediatR(Assembly.GetExecutingAssembly());

            //define pipeline behaviours
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

            return services;
        }
    }
}
