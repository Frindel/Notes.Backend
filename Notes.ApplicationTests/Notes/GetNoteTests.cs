using Notes.Application.Common.Exceptions;
using Notes.Application.Notes.Queries.GetNote;
using Notes.ApplicationTests.Common;
using Notes.Domain;
using Notes.Persistence.Data;

namespace Notes.ApplicationTests.Notes
{
    [TestFixture]
    internal class GetNoteTests : TestsBase
    {

        [Test]
        public async Task SuccessfullGettingNotes()
        {
            // Arrange
            DataContext context = ContextManager.CreateEmptyDataContex();
            User savedUser = Helper.AddUserWithNumbers(context, 1).First();
            List<Category> savedCategories = Helper.AddCategoriesWithNumbers(context, savedUser, 1, 2);
            Note savedNote = Helper.AddNotesWithNumbers(context, savedUser, savedCategories, 1).First();

            var heandler = CreateQueryHeandler(context);
            var query = CreateQuery(savedUser, savedNote);

            // Act
            var getedNote = await heandler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(getedNote, "result is null");
                Assert.IsNotNull(getedNote.Name, "name in result is empty");
                Assert.IsNotNull(getedNote.Description, "description in result is empty");
                Assert.IsTrue(getedNote.Categories.Count == savedCategories.Count,
                    "number of categories in the result does not match");
            });
        }

        [Test]
        public void UserIsNotFoundException()
        {
            // Arrange
            DataContext context = ContextManager.CreateEmptyDataContex();
            User notSavedUser = Helper.CreateUserOfNumber(1);
            Note notSavedNote = Helper.CreateNoteOfNumber(1, notSavedUser, new List<Category>());

            var heandler = CreateQueryHeandler(context);
            var query = CreateQuery(notSavedUser, notSavedNote);

            // Act / Assert
            Assert.ThrowsAsync<UserNotFoundException>(() => heandler.Handle(query, CancellationToken.None));
        }

        [Test]
        public void NoteIsNotFoundException()
        {
            // Arrange
            DataContext context = ContextManager.CreateEmptyDataContex();
            User savedUser = Helper.AddUserWithNumbers(context, 1).First();
            Note notSavedNote = Helper.CreateNoteOfNumber(1, savedUser, new List<Category>());

            var heandler = CreateQueryHeandler(context);
            var query = CreateQuery(savedUser, notSavedNote);

            // Act / Assert
            Assert.ThrowsAsync<NoteNotFoundException>(() => heandler.Handle(query, CancellationToken.None));
        }

        GetNoteQueryHeandler CreateQueryHeandler(DataContext context)
        {
            return new GetNoteQueryHeandler(context, context, Mapper);
        }

        GetNoteQuery CreateQuery(User user, Note note)
        {
            return new GetNoteQuery()
            {
                UserId = user.Id,
                NoteId = note.PersonalId
            };
        }
    }
}
