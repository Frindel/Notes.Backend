using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notes.Application.Interfaces;
using Notes.Persistence.Data;

namespace Notes.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection OnPersistence(this IServiceCollection services,
            IConfiguration configuration)
        {
            AddDbService(services, configuration);

            // регистрация реализаций интерфейса уровня приложения
            services.AddScoped<IUsersContext>(options => options.GetService<DataContext>()!);
            services.AddScoped<ICategoriesContext>(options => options.GetService<DataContext>()!);
            services.AddScoped<INotesContext>(options => options.GetService<DataContext>()!);

            return services;
        }

        private static void AddDbService(IServiceCollection services, IConfiguration configuration)
        {
            string? dbConnectionString = configuration["ConnectionStrings:DbConnection"];

            if (dbConnectionString == null)
                throw new ArgumentNullException(nameof(dbConnectionString));

            services.AddDbContext<DataContext>(options =>
            {
                options.UseNpgsql(dbConnectionString);
            });
        }
    }
}
