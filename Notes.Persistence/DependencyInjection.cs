using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notes.Application.Interfaces;

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

            // todo: регистрация сервиса получения id текущего пользователя


            return services;
        }

        private static void AddDbService(IServiceCollection services, IConfiguration configuration)
        {
            string? dbConnectionString = configuration["dbConnection"];

            if (dbConnectionString == null)
                throw new ArgumentNullException(nameof(dbConnectionString));

            services.AddDbContext<DataContext>(options =>
            {
                options.UseNpgsql(dbConnectionString);
            });
        }
    }
}
