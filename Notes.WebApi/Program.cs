using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Notes.Application;
using Notes.Application.Common.Mapping;
using Notes.Persistence;
using System.Reflection;
using System.Text;
using Notes.WebApi.Middleware;

namespace Notes.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ServiceConfiguration(builder);

            var app = builder.Build();
            MiddlewareConfiguration(app);
            app.Run();
        }

        #region Application builder settings
        
        static WebApplicationBuilder ServiceConfiguration(WebApplicationBuilder builder)
        {
            var services = builder.Services;
            var configuration = builder.Configuration;
            ConnectDependencies(services, configuration);
            RegisterMappers(services);
            SetJwtAuthentication(services, configuration);
            SetSwaggerSettings(services);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors();

            return builder;
        }

        static IServiceCollection ConnectDependencies(IServiceCollection services, ConfigurationManager configuration)
        {
            services.OnApplication();
            services.OnPersistence(configuration);
            return services;
        }

        static IServiceCollection RegisterMappers(IServiceCollection services)
        {
            services.AddAutoMapper(config =>
            {
                config.AddProfile(new AssemblyMappingProfile(Assembly.GetExecutingAssembly()));
                config.AddProfile(new AssemblyMappingProfile(typeof(AssemblyMappingProfile).Assembly));
            });
            return services;
        }

        static IServiceCollection SetJwtAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthorization();
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true, // валидация издателя
                        ValidIssuer = configuration["Jwt:Issuer"], // издатель
                        ValidateAudience = true, // валидация потребителя токена
                        ValidAudience = configuration["Jwt:Audience"], // потребитель
                        ValidateLifetime = true, // валидация времени существования
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!)), // ключ безопасности
                        ValidateIssuerSigningKey = true, // валидация ключа безопасности
                    };
                });
            return services;
        }

        static IServiceCollection SetSwaggerSettings(IServiceCollection services) =>
            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "Pathnostics", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

        #endregion
        
        #region Middleware settings
        
        static WebApplication MiddlewareConfiguration(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(options =>
                options.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());

            app.UseHttpsRedirection();
            app.UseMiddleware<ExceptionsHandlerMiddleware>();
            app.UseAuthorization(); 
            app.MapControllers();

            return app;
        }
        
        #endregion
    }
}