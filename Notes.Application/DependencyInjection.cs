using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Notes.Application.Common.Behaviors;
using System.Reflection;
using Notes.Application.Common.Helpers;

namespace Notes.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection OnApplication(this IServiceCollection services)
        {
            services.AddMediatR(options => { options.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()); });
            services.AddValidatorsFromAssemblies(new[] { Assembly.GetExecutingAssembly() });
            SetBehaviors(services);
            RegisterHelpers(services);
            return services;
        }

        static IServiceCollection SetBehaviors(IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(OperationsLockingBehavior<,>));
            return services;
        }
        
        static IServiceCollection RegisterHelpers(IServiceCollection services)
        {
            services.AddScoped<UsersHelper>();
            services.AddScoped<NotesHelper>();
            services.AddScoped<CategoriesHelper>();

            return services;
        }
    }
}