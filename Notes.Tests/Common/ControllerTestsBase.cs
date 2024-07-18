using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Notes.Application.Common.Behaviors;
using Notes.Application.Common.Helpers;
using Notes.Application.Common.Mapping;
using Notes.Application.Interfaces;
using Notes.ApplicationTests.Common;
using Notes.Persistence;
using Notes.Persistence.Data;
using Notes.WebApi.Controllers;
using System.Reflection;

namespace Notes.Tests.Common
{
    internal class ControllerTestsBase<TypeController> : TestsBase
        where TypeController : BaseController
    {
        readonly Assembly _applicationLevelAssembly;

        public ControllerTestsBase()
        {
            _applicationLevelAssembly = typeof(INotesContext).Assembly;
        }

        protected TypeController CreateController(DataContext dataContext)
        {
            HttpContext controllerContext = CreateControllerContext(dataContext);
            var controller = Activator.CreateInstance<TypeController>();
            controller.ControllerContext = new()
            {
                HttpContext = controllerContext
            };
            return controller;
        }

        HttpContext CreateControllerContext(DataContext dataContext)
        {
            var services = ConfiguringServices(dataContext);
            var context = new DefaultHttpContext();
            context.RequestServices = services.BuildServiceProvider();
            return context;
        }

        ServiceCollection ConfiguringServices(DataContext dataContext)
        {
            var services = new ServiceCollection();
            RegistrationFoApplicationLevelService(services);
            RegistrationFoPersistenceLevelService(services, dataContext);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            return services;
        }

        ServiceCollection RegistrationFoApplicationLevelService(ServiceCollection services)
        {
            services.AddMediatR(options =>
            {
                options.RegisterServicesFromAssembly(_applicationLevelAssembly);
            });
            services.AddValidatorsFromAssemblies(new[] { _applicationLevelAssembly });
            services.AddAutoMapper(options =>
            {
                options.AddProfile(new AssemblyMappingProfile(_applicationLevelAssembly));
            });
            services.AddScoped<UsersHelper>();
            services.AddScoped<NotesHelper>();
            services.AddScoped<CategoriesHelper>();

            return services;
        }

        ServiceCollection RegistrationFoPersistenceLevelService(ServiceCollection services, DataContext dataContext)
        {
            services.AddScoped(_ => dataContext);
            services.AddScoped<IUsersContext>(options => options.GetService<DataContext>()!);
            services.AddScoped<ICategoriesContext>(options => options.GetService<DataContext>()!);
            services.AddScoped<INotesContext>(options => options.GetService<DataContext>()!);
            services.AddScoped(_ => CreateJwtTokensServiceMoq(services));
            return services;
        }

        IJwtTokensService CreateJwtTokensServiceMoq(ServiceCollection services)
        {
            var jwtTokensSerivceMock = new Mock<IJwtTokensService>();
            jwtTokensSerivceMock
                .Setup(s=>s.TokenIsValid(It.IsAny<string>()))
                .Returns(true);
            jwtTokensSerivceMock
                .Setup(s => s.GenerateAccessToken(It.IsAny<int>()))
                .Returns(Helper.CreateRandomStr(256));
            jwtTokensSerivceMock
                .Setup(s => s.GenerateRefrechToken(It.IsAny<int>()))
                .Returns(Helper.CreateRandomStr(256));
            return jwtTokensSerivceMock.Object;
        }
    }
}
