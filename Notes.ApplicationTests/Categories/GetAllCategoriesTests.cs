using Notes.Application.Categories.Command.Queries.GetAllCategories;
using Notes.Application.Common.Exceptions;
using Notes.ApplicationTests.Common;
using Notes.Domain;
using Notes.Persistence.Data;

namespace Notes.ApplicationTests.Categories
{
    [TestFixture]
    internal class GetAllCategoriesTests : TestBase
    {
        [Test]
        public async Task SuccessfulGettingCategories()
        {
            // Arrange
            User firstUser = Helper.CreateUserOfNumber(1);
            Category firstCategory = Helper.CreateCategoryOfNumber(1, firstUser);
            Category secondCategory = Helper.CreateCategoryOfNumber(2, firstUser);

            DataContext context = CreateEmptyDataContex();
            context.Users.Add(firstUser);
            context.Categories.AddRange(firstCategory, secondCategory);
            await context.SaveChangesAsync();

            var heandler = new GetAllCategoriesQueryHeandler(context, context, Mapper);
            var query = new GetAllCategoriesQuery()
            {
                UserId = firstUser.Id
            };

            // Act
            var categories = await heandler.Handle(query, CancellationToken.None);

            // Accert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(categories);
                Assert.IsNotNull(categories.Categories);
                Assert.IsNotEmpty(categories.Categories);
                Assert.IsTrue(categories.Categories.Count == context.Categories.Count());
            });
        }

        [Test]
        public void UserNotFoundException()
        {
            // Arrange
            User firstUser = Helper.CreateUserOfNumber(1);
            User secondNotSavedUser = Helper.CreateUserOfNumber(2);
            
            DataContext context = CreateEmptyDataContex();
            context.Users.Add(firstUser);
            context.SaveChanges();

            var heandler = new GetAllCategoriesQueryHeandler(context, context, Mapper);
            var query = new GetAllCategoriesQuery()
            {
                UserId = secondNotSavedUser.Id
            };

            // Act / Accert
            Assert.ThrowsAsync<UserNotFoundException>(() => heandler.Handle(query, CancellationToken.None));

        }
    }
}
