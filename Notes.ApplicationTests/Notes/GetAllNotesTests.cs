using Notes.Application.Common.Exceptions;
using Notes.Application.Notes.Queries.GetAllNotes;
using Notes.ApplicationTests.Common;
using Notes.Domain;
using Notes.Persistence.Data;

namespace Notes.ApplicationTests.Notes
{
    [TestFixture]
    internal class GetAllNotesTests : TestsBase
    {
        [Test]
        public async Task SuccessfulGetingNotes()
        {
            // Arrange
            DataContext context = ContextManager.CreateEmptyDataContex();
            User savedUser = Helper.AddUserWithNumbers(context, 1).First();
            List<Category> savedCategories = Helper.AddCategoriesWithNumbers(context, savedUser, 1, 2);
            List<Note> notes = Helper.AddNotesWithNumbers(context, savedUser, savedCategories, 1, 2, 3);

            var heandler = CreateQueryHeandler(context);
            var query = CreateQuery(savedUser);

            // Act
            var getedUsers = await heandler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(getedUsers, "result is null");
                Assert.IsTrue(getedUsers.Notes.Count == notes.Count,
                    "number of notes in the result does not match");
            });
        }

        [Test]
        public void UserNotFoundException()
        {
            // Arrange
            DataContext context = ContextManager.CreateEmptyDataContex();
            User notSavedUser = Helper.CreateUserOfNumber(1);

            var heandler = CreateQueryHeandler(context);
            var query = CreateQuery(notSavedUser);

            // Act / Assert
            Assert.ThrowsAsync<UserNotFoundException>(() => heandler.Handle(query, CancellationToken.None));
        }

        GetAllNotesQueryHeandler CreateQueryHeandler(DataContext context)
        {
            return new GetAllNotesQueryHeandler(context, context, Mapper);
        }

        GetAllNotesQuery CreateQuery(User user)
        {
            return new GetAllNotesQuery()
            {
                UserId = user.Id
            };
        }
    }
}
