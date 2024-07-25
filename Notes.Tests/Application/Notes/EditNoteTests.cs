using Moq;
using Notes.Application.Common.Exceptions;
using Notes.Application.Common.Helpers;
using Notes.Application.Interfaces;
using Notes.Application.Notes.Commands.EditNote;
using Notes.Application.Notes.Dto;
using Notes.ApplicationTests.Common;
using Notes.Domain;
using Notes.Persistence.Data;

namespace Notes.Tests.Application.Notes
{
    [TestFixture]
    internal class EditNoteTests : TestsBase
    {
        DataContext _context;
        User _savedUser;
        List<Category> _savedCategories;
        Note _savedNote;
        EditNoteCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _context = ContextManager.CreateEmptyDataContex();
            _savedUser = Helper.AddUserWithNumbers(_context, 1).First();
            _savedCategories = Helper.AddCategoriesWithNumbers(_context, _savedUser, 1, 2);
            _savedNote = Helper.AddNotesWithNumbers(_context, _savedUser, null!, 1).First();
            _handler = CreateHandler();
        }

        EditNoteCommandHandler CreateHandler()
        {
            var jwtTokensServiceMock = new Mock<IJwtTokensService>();
            UsersHelper usersHelper = new UsersHelper(_context, jwtTokensServiceMock.Object);
            CategoriesHelper categoriesHelper = new CategoriesHelper(_context);
            NotesHelper notesHelper = new NotesHelper(_context);

            var handler = new EditNoteCommandHandler(usersHelper, categoriesHelper, notesHelper, Mapper);
            return handler;
        }

        [Test]
        public async Task SuccessfulEditWithoutSpecifiedCategories()
        {
            // Arrange
            Note changedNote = CreateChangedNote(_savedNote, new List<Category>());
            var command = CreateCommand(_savedUser, changedNote);

            // Act
            NoteDto updatedNote = await _handler.Handle(command, CancellationToken.None);

            // Accert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(updatedNote, "result is null");
                Assert.IsNotNull(updatedNote.Name, "result name is null");
                Assert.IsNotNull(updatedNote.Description, "result description is null");
                Assert.IsTrue(updatedNote.Categories.Count == changedNote.Categories.Count,
                    "number of categories in the result does not match");
            });
        }

        [Test]
        public async Task SuccessfullyChangingNoteWithoutChangingCategories()
        {
            // Arrange
            Note changedNote = CreateChangedNote(_savedNote, _savedCategories);
            var command = CreateCommand(_savedUser, changedNote);

            // Act
            NoteDto updatedNote = await _handler.Handle(command, CancellationToken.None);

            // Accert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(updatedNote, "result is null");
                Assert.IsNotNull(updatedNote.Name, "result name is null");
                Assert.IsNotNull(updatedNote.Description, "result description is null");
                Assert.IsTrue(updatedNote.Categories.Count == changedNote.Categories.Count,
                    "number of categories in the result does not match");
            });
        }

        // успешное с изменением категорий
        [Test]
        public async Task SuccessfulEditNoteWithCategoryChanges()
        {
            // Arrange
            Note changedNote = CreateChangedNote(_savedNote, new List<Category>());
            var command = CreateCommand(_savedUser, changedNote);

            // Act
            NoteDto updatedNote = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(updatedNote, "result is null");
                Assert.IsNotNull(updatedNote.Name, "result name is null");
                Assert.IsNotNull(updatedNote.Description, "result description is null");
                Assert.IsTrue(updatedNote.Categories.Count == changedNote.Categories.Count,
                    "number of categories in the result does not match");
            });
        }

        // пользователь не найден
        [Test]
        public void UserNotFoundException()
        {
            // Arrange
            User notSavedUser = Helper.CreateUserOfNumber(2);
            Note notSavedNote = Helper.CreateNoteOfNumber(2, notSavedUser, new List<Category>());
            var command = CreateCommand(notSavedUser, notSavedNote);

            // Act / Assert
            Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }

        // заметка не найдена
        [Test]
        public void NoteNotFoundException()
        {
            // Arrange
            Note notSavedNote = Helper.CreateNoteOfNumber(2, _savedUser, new List<Category>());
            var command = CreateCommand(_savedUser, notSavedNote);

            // Act / Assert
            Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }

        // категория не найдена
        [Test]
        public void CategoryNotFoundException()
        {
            // Arrange
            Category notSavedCategory = Helper.CreateCategoryOfNumber(3, _savedUser);
            Note updatedNote = CreateChangedNote(_savedNote, new List<Category>() { notSavedCategory });
            var command = CreateCommand(_savedUser, updatedNote);

            // Act / Assert
            Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));

        }

        EditNoteCommand CreateCommand(User user, Note note)
        {
            List<int> categoriesIds = note.Categories.Select(c => c.PersonalId).ToList();

            var command = new EditNoteCommand()
            {
                NoteId = note.PersonalId,
                UserId = user.Id,

                Name = note.Name,
                Description = note.Description,
                Time = note.Time,
                IsCompleted = note.IsCompleted,
                CategoriesIds = categoriesIds
            };
            return command;
        }

        Note CreateChangedNote(Note note, List<Category> categories)
        {
            return new Note()
            {
                Id = note.Id,
                PersonalId = note.PersonalId,
                Name = $"changed {note.Name}",
                Description = $"changed {note.Description}",
                IsCompleted = note.IsCompleted,
                Time = note.Time,
                User = note.User,
                Categories = categories
            };
        }
    }
}
