using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Notes.Application.Common.Behaviors;
using System.Reflection;

namespace Notes.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection OnApplication(this IServiceCollection services)
        {
            services.AddMediatR(options =>
            {
                options.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });

            // регистрация валидаторов
            services.AddValidatorsFromAssemblies(new[] { Assembly.GetExecutingAssembly() });

            // регистарция промежутночны обработчиков запроса
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }

    }
}
