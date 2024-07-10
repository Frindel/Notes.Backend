using Notes.Application.Categories.Command.Queries.GetCategory;
using Notes.Application.Common.Exceptions;
using Notes.ApplicationTests.Common;
using Notes.Domain;
using Notes.Persistence.Data;

namespace Notes.ApplicationTests.Categories
{
    [TestFixture]
    internal class GetCategoryTests : TestBase
    {
        const string _cateroryName = "test";

        [Test]
        public async Task SuccessfulGettingUser()
        {
            // Arrange
            User firstTestUser = Helper.CreateUserOfNumber(1);
            Category testCategory = new Category()
            {
                Name = _cateroryName,
                User = firstTestUser
            };

            DataContext context = CreateEmptyDataContex();
            context.Users.Add(firstTestUser);
            context.Categories.Add(testCategory);
            await context.SaveChangesAsync();

            var query = new GetCategoryQuery()
            {
                CategoryId = testCategory.Id,
                UserId = firstTestUser.Id
            };
            var heandler = new GetCategoryQueryHeandler(context, context, Mapper);

            // Act
            var category = await heandler.Handle(query, CancellationToken.None);

            // Accert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(category);
                Assert.IsNotNull(category.Name);
            });
        }

        [Test]
        public void UserNotFoundException()
        {
            // Arrange
            User firstTestUser = Helper.CreateUserOfNumber(1);
            User notSavedUser = Helper.CreateUserOfNumber(2);

            DataContext context = CreateEmptyDataContex();
            context.Users.Add(firstTestUser);
            context.SaveChanges();

            var query = new GetCategoryQuery()
            {
                UserId = notSavedUser.Id,
                CategoryId = 1
            };
            var heandler = new GetCategoryQueryHeandler(context, context, Mapper);

            // Act / Accert
            Assert.ThrowsAsync<UserNotFoundException>(()=>heandler.Handle(query, CancellationToken.None));
        }

        [Test]
        public void CategoryNotFoundException()
        {
            // Arrange
            User firstTestUser = Helper.CreateUserOfNumber(2);
            Category firstCategory = Helper.CreateCategoryOfNumber(1, firstTestUser);
            Category secondNotFoundCategory = Helper.CreateCategoryOfNumber(2, firstTestUser);

            DataContext context = CreateEmptyDataContex();
            context.Users.Add(firstTestUser);
            context.Categories.Add(firstCategory);
            context.SaveChanges();

            var query = new GetCategoryQuery()
            {
                UserId = firstTestUser.Id,
                CategoryId = secondNotFoundCategory.Id
            };
            var heandler = new GetCategoryQueryHeandler(context, context, Mapper);

            // Act / Accert
            Assert.ThrowsAsync<CategoryNotFoundException>(() => heandler.Handle(query, CancellationToken.None));
        }
    }
}
