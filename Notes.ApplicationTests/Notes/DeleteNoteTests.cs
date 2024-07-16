using Moq;
using Notes.Application.Common.Exceptions;
using Notes.Application.Common.Helpers;
using Notes.Application.Interfaces;
using Notes.Application.Notes.Commands.DeleteNote;
using Notes.ApplicationTests.Common;
using Notes.Domain;
using Notes.Persistence.Data;

namespace Notes.ApplicationTests.Notes
{
    [TestFixture]
    internal class DeleteNoteTests : TestsBase
    {
        DataContext _context;
        User _savedUser;
        List<Category> _savedCategories;
        Note _savedNote;
        DeleteNoteCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _context = ContextManager.CreateEmptyDataContex();
            _savedUser = Helper.AddUserWithNumbers(_context, 1).First();
            _savedCategories = Helper.AddCategoriesWithNumbers(_context, _savedUser, 1, 2);
            _savedNote = Helper.AddNotesWithNumbers(_context, _savedUser, _savedCategories, 1).First();
            _handler = CreateHandler();
        }

        DeleteNoteCommandHandler CreateHandler()
        {
            var jwtTokensServiceMock = new Mock<IJwtTokensService>();
            UsersHelper usersHelper = new UsersHelper(_context, jwtTokensServiceMock.Object);
            NotesHelper notesHelper = new NotesHelper(_context);

            var handler = new DeleteNoteCommandHandler(usersHelper, notesHelper, _context);
            return handler;
        }

        [Test]
        public void SuccessfulDeleteUser()
        {
            // Arrange
            var command = CreateCommand(_savedUser, _savedNote);

            // Act / Accert
            Assert.IsTrue(_handler.Handle(command, CancellationToken.None).IsCompletedSuccessfully);
        }

        [Test]
        public void UserIsNotFoundException()
        {
            // Arrange
            User notSavedUser = Helper.CreateUserOfNumber(2);
            Note notSavedNote = Helper.CreateNoteOfNumber(2, notSavedUser);
            var command = CreateCommand(notSavedUser, notSavedNote);

            // Act / Accert
            Assert.ThrowsAsync<UserNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Test]
        public void NoteIsNotFoundException()
        {
            // Arrange
            Note notSavedNote = Helper.CreateNoteOfNumber(2, _savedUser);
            var command = CreateCommand(_savedUser, notSavedNote);

            // Act / Accert
            Assert.ThrowsAsync<NoteNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }

        DeleteNoteCommand CreateCommand(User user, Note note)
        {
            var command = new DeleteNoteCommand()
            {
                NoteId = note.PersonalId,
                UserId = user.Id
            };
            return command;
        }
    }
}
