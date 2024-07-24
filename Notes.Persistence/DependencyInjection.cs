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

            AddJwtTokensService(services, configuration);
            
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

        static void AddJwtTokensService(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IJwtTokensService>(_ => new JwtTokensService(
               secret: configuration["Jwt:Secret"]!,
               issuer: configuration["Jwt:Issuer"]!,
               audience: configuration["Jwt:Audience"]!,
               accessTokenLiveTimeSeconds: int.Parse(configuration["Jwt:AccessTokenValidity"]!),
               refreshTokenLiveTimeSeconds: int.Parse(configuration["Jwt:RefreshTokenValidity"]!)));
        }
    }
}
