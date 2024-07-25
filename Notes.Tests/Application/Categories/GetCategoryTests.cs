using Moq;
using Notes.Application.Categories.Queries.GetCategory;
using Notes.Application.Common.Exceptions;
using Notes.Application.Common.Helpers;
using Notes.Application.Interfaces;
using Notes.ApplicationTests.Common;
using Notes.Domain;
using Notes.Persistence.Data;

namespace Notes.Tests.Application.Categories
{
    [TestFixture]
    internal class GetCategoryTests : TestsBase
    {
        DataContext _context;
        User _savedUser;
        Category _savedCategory;
        GetCategoryQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _context = ContextManager.CreateEmptyDataContex();
            _savedUser = Helper.AddUserWithNumbers(_context, 1).First();
            _savedCategory = Helper.AddCategoriesWithNumbers(_context, _savedUser, 1).First();
            _handler = CreateHandler();
        }

        GetCategoryQueryHandler CreateHandler()
        {
            var jwtTokensServiceMock = new Mock<IJwtTokensService>();
            UsersHelper usersHelper = new UsersHelper(_context, jwtTokensServiceMock.Object);

            var handler = new GetCategoryQueryHandler(usersHelper, _context, _context, Mapper);
            return handler;
        }

        [Test]
        public async Task GetCategory_Success()
        {
            // Arrange
            var query = CreateQuery(_savedCategory, _savedUser);

            // Act
            var category = await _handler.Handle(query, CancellationToken.None);

            // Accert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(category);
                Assert.IsNotNull(category.Name);
            });
        }

        [Test]
        public void GetCategory_InvalidUser_ThrowsNotFoundException()
        {
            // Arrange
            User notSavedUser = Helper.CreateUserOfNumber(2);
            var query = CreateQuery(_savedCategory, notSavedUser);

            // Act / Accert
            Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(query, CancellationToken.None));
        }

        [Test]
        public void GetCategory_InvalidCategory_ThrowsNotFoundException()
        {
            // Arrange
            Category notSavedCategory = Helper.CreateCategoryOfNumber(2, _savedUser);
            var query = CreateQuery(notSavedCategory, _savedUser);

            // Act / Accert
            Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(query, CancellationToken.None));
        }

        public GetCategoryQuery CreateQuery(Category category, User forUser)
        {
            return new GetCategoryQuery()
            {
                CategoryId = category.PersonalId,
                UserId = forUser.Id
            };
        }
    }
}