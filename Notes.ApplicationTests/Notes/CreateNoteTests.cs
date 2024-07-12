using Notes.Application.Common.Exceptions;
using Notes.Application.Notes.Commands.CreateNote;
using Notes.ApplicationTests.Common;
using Notes.Domain;
using Notes.Persistence.Data;

namespace Notes.ApplicationTests.Notes
{
    [TestFixture]
    internal class CreateNoteTests : TestBase
    {
        [Test]
        public async Task SuccessCreatedNote()
        {
            // Arrange
            DataContext context = CreateEmptyDataContex();
            List<User> users = AddUserWithNumbers(context, 1);
            List<Category> categories = AddCategoriesWithNumbers(context, users.First(), 1, 2);

            var heandler = CreateHeandler(context);
            var command = CreateCommand(users.First(), categories);

            // Act
            var createdNote = await heandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(createdNote, "result is null");
                Assert.IsNotNull(createdNote.Name, "created note name is not sated");
                Assert.IsNotNull(createdNote.Description, "created note description is not sated");
                Assert.IsTrue(createdNote.Categories.Count() == command.CategoriesIds.Count(), "sated categories is not saved");
            });
        }

        [Test]
        public void UserNotFoundException()
        {
            // Arrange
            DataContext context = CreateEmptyDataContex();
            User notSavedUser = CreateUserOfNumber(1);

            var heandler = CreateHeandler(context);
            var command = CreateCommand(notSavedUser);

            // Act / accert
            Assert.ThrowsAsync<UserNotFoundException>(() => heandler.Handle(command, CancellationToken.None));
        }

        [Test]
        public void CategoryNotFoundException()
        {
            // Arrange
            DataContext context = CreateEmptyDataContex();
            List<User> savedUsers = AddUserWithNumbers(context, 1);
            Category notSavedCategory = CreateCategoryOfNumber(1, savedUsers.First());

            var heandler = CreateHeandler(context);
            var command = CreateCommand(savedUsers.First(), new List<Category>() { notSavedCategory });

            // Act / accert
            Assert.ThrowsAsync<CategoryNotFoundException>(() => heandler.Handle(command, CancellationToken.None));
        }

        CreateNoteCommandHeandler CreateHeandler(DataContext context)
        {
            var heandler = new CreateNoteCommandHeandler(context, context, context, Mapper);
            return heandler;
        }

        CreateNoteCommand CreateCommand(User forUser, List<Category> withCategories = null!)
        {
            List<int> categoriesIds = (withCategories ?? new List<Category>()).Select(c => c.PersonalId).ToList();

            var command = new CreateNoteCommand()
            {
                UserId = forUser.Id,
                CategoriesIds = categoriesIds,
                Name = "test note name",
                Description = "test note description"
            };
            return command;
        }
    }
}
