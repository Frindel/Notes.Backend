using Moq;
using Notes.Application.Common.Exceptions;
using Notes.Application.Common.Helpers;
using Notes.Application.Interfaces;
using Notes.Application.Notes.Queries.GetNote;
using Notes.ApplicationTests.Common;
using Notes.Domain;
using Notes.Persistence.Data;

namespace Notes.ApplicationTests.Notes
{
    [TestFixture]
    internal class GetNoteTests : TestsBase
    {
        DataContext _context;
        User _savedUser;
        List<Category> _savedCategories;
        Note _savedNote;
        GetNoteQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _context = ContextManager.CreateEmptyDataContex();
            _savedUser = Helper.AddUserWithNumbers(_context, 1).First();
            _savedCategories = Helper.AddCategoriesWithNumbers(_context, _savedUser, 1, 2);
            _savedNote = Helper.AddNotesWithNumbers(_context, _savedUser, _savedCategories, 1).First();
            _handler = CreateHandler();
        }

        GetNoteQueryHandler CreateHandler()
        {
            var jwtTokensServiceMock = new Mock<IJwtTokensService>();
            UsersHelper usersHelper = new UsersHelper(_context, jwtTokensServiceMock.Object);
            NotesHelper notesHelper = new NotesHelper(_context);

            return new GetNoteQueryHandler(usersHelper, notesHelper, Mapper);
        }

        [Test]
        public async Task SuccessfullGettingNotes()
        {
            // Arrange
            var query = CreateQuery(_savedUser, _savedNote);

            // Act
            var retrievedNote = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(retrievedNote, "result is null");
                Assert.IsNotNull(retrievedNote.Name, "name in result is empty");
                Assert.IsNotNull(retrievedNote.Description, "description in result is empty");
                Assert.IsTrue(retrievedNote.Categories.Count == _savedCategories.Count,
                    "number of categories in the result does not match");
            });
        }

        [Test]
        public void UserIsNotFoundException()
        {
            // Arrange
            User notSavedUser = Helper.CreateUserOfNumber(2);
            Note notSavedNote = Helper.CreateNoteOfNumber(1, notSavedUser, new List<Category>());
            var query = CreateQuery(notSavedUser, notSavedNote);

            // Act / Assert
            Assert.ThrowsAsync<UserNotFoundException>(() => _handler.Handle(query, CancellationToken.None));
        }

        [Test]
        public void NoteIsNotFoundException()
        {
            // Arrange
            Note notSavedNote = Helper.CreateNoteOfNumber(2, _savedUser, new List<Category>());
            var query = CreateQuery(_savedUser, notSavedNote);

            // Act / Assert
            Assert.ThrowsAsync<NoteNotFoundException>(() => _handler.Handle(query, CancellationToken.None));
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
