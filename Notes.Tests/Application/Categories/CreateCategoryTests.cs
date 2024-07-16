using Moq;
using Notes.Application.Categories.Commands.CreateCategory;
using Notes.Application.Common.Exceptions;
using Notes.Application.Common.Helpers;
using Notes.Application.Interfaces;
using Notes.ApplicationTests.Common;
using Notes.Domain;
using Notes.Persistence.Data;

namespace Notes.Tests.Application.Categories
{
    [TestFixture]
    internal class CreateCategoryTests : TestsBase
    {
        DataContext _context;
        User _sevedUser;
        CreateCategoryCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _context = ContextManager.CreateEmptyDataContex();
            _sevedUser = Helper.AddUserWithNumbers(_context, 1).First();
            _handler = CreateHandler();
        }

        CreateCategoryCommandHandler CreateHandler()
        {
            var jwtTokensServiceMock = new Mock<IJwtTokensService>();
            UsersHelper usersHelper = new UsersHelper(_context, jwtTokensServiceMock.Object);
            CategoriesHelper categoriesHelper = new CategoriesHelper(_context);

            var handler = new CreateCategoryCommandHandler(usersHelper, categoriesHelper, _context, Mapper);
            return handler;
        }

        [Test]
        public async Task SuccessCategoryCreated()
        {
            // Arrange
            var command = CreateCommand(_sevedUser);

            // Act
            var createCategory = await _handler.Handle(command, CancellationToken.None);

            // Accert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(createCategory);
                Assert.IsNotNull(createCategory.Name);
            });
        }

        [Test]
        public async Task UserNotFoundException()
        {
            // Arrange
            User notSavedUser = Helper.CreateUserOfNumber(2);
            var command = CreateCommand(notSavedUser);

            // Act / Assert
            Assert.ThrowsAsync<UserNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }

        public CreateCategoryCommand CreateCommand(User forUser)
        {
            return new CreateCategoryCommand()
            {
                CategoryName = "new category",
                UserId = forUser.Id
            };
        }
    }
}
