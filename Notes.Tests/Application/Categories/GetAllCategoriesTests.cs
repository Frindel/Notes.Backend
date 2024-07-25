using Moq;
using Notes.Application.Categories.Queries.GetAllCategories;
using Notes.Application.Common.Exceptions;
using Notes.Application.Common.Helpers;
using Notes.Application.Interfaces;
using Notes.ApplicationTests.Common;
using Notes.Domain;
using Notes.Persistence.Data;

namespace Notes.Tests.Application.Categories
{
    [TestFixture]
    internal class GetAllCategoriesTests : TestsBase
    {
        DataContext _context;
        User _savedUser;
        List<Category> _savedCategories;
        GetAllCategoriesQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _context = ContextManager.CreateEmptyDataContex();
            _savedUser = Helper.AddUserWithNumbers(_context, 1).First();
            _savedCategories = Helper.AddCategoriesWithNumbers(_context, _savedUser, 1, 2);
            _handler = CreateHandler();
        }

        GetAllCategoriesQueryHandler CreateHandler()
        {
            var jwtTokensServiceMock = new Mock<IJwtTokensService>();
            UsersHelper usersHelper = new UsersHelper(_context, jwtTokensServiceMock.Object);

            var handler = new GetAllCategoriesQueryHandler(usersHelper, _context, Mapper);
            return handler;
        }

        [Test]
        public async Task GetAllCategories_Success()
        {
            // Arrange
            var query = CreateQuery(_savedUser);

            // Act
            var categories = await _handler.Handle(query, CancellationToken.None);

            // Accert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(categories);
                Assert.IsNotNull(categories.Categories);
                Assert.IsNotEmpty(categories.Categories);
                Assert.IsTrue(categories.Categories.Count == _savedCategories.Count,
                    "number of categories in the result does not match");
            });
        }

        [Test]
        public void GetAllCategories_InvalidUser_ThrowsNotFoundException()
        {
            // Arrange
            User notSavedUser = Helper.CreateUserOfNumber(2);
            var query = CreateQuery(notSavedUser);

            // Act / Accert
            Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(query, CancellationToken.None));
        }

        public GetAllCategoriesQuery CreateQuery(User forUser)
        {
            return new GetAllCategoriesQuery()
            {
                UserId = forUser.Id
            };
        }
    }
}