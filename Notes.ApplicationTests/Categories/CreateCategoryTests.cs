using Notes.Application.Categories.Command.CreateCategory;
using Notes.Application.Common.Exceptions;
using Notes.ApplicationTests.Common;
using Notes.Domain;
using Notes.Persistence.Data;

namespace Notes.ApplicationTests.Categories
{
    [TestFixture]
    internal class CreateCategoryTests : TestsBase
    {
        const string categoryName = "test";

        [Test]
        public async Task SuccessCategoryCreated()
        {
            // Arrange
            User testUser = Helper.CreateUserOfNumber(1);
            DataContext context = ContextManager.CreateEmptyDataContex();
            context.Users.Add(testUser);
            await context.SaveChangesAsync();

            var heandler = new CreateCategoryCommandHeandler(context, context, Mapper);

            var command = new CreateCategoryCommand()
            {
                CategoryName = categoryName,
                UserId = testUser.Id
            };

            // Act
            var createCategory = await heandler.Handle(command, CancellationToken.None);

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
            User firstTestUser = Helper.CreateUserOfNumber(1);
            User secondTestUser = Helper.CreateUserOfNumber(2);
            DataContext context = ContextManager.CreateEmptyDataContex();
            context.Users.AddRange(firstTestUser);
            await context.SaveChangesAsync();

            var heandler = new CreateCategoryCommandHeandler(context, context, Mapper);

            var command = new CreateCategoryCommand()
            {
                CategoryName = categoryName,
                UserId = secondTestUser.Id
            };

            // Act / Assert
            Assert.ThrowsAsync<UserNotFoundException>(() => heandler.Handle(command, CancellationToken.None));
        }
    }
}
