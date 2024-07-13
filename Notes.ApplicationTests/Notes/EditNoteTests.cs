using Notes.Application.Common.Exceptions;
using Notes.Application.Notes.Commands.EditNote;
using Notes.Application.Notes.Dto;
using Notes.ApplicationTests.Common;
using Notes.Domain;
using Notes.Persistence.Data;

namespace Notes.ApplicationTests.Notes
{
    [TestFixture]
    internal class EditNoteTests : TestsBase
    {
        // успешное без категорий
        [Test]
        public async Task SuccessfulEditWithoutSpecifiedCategories()
        {
            // Arrange
            DataContext context = ContextManager.CreateEmptyDataContex();
            User savedUser = Helper.AddUserWithNumbers(context, 1).First();
            Note savedNote = Helper.AddNotesWithNumbers(context, savedUser, new List<Category>(), 1).First();

            Note changedNote = CreateChangedNote(savedNote, new List<Category>());

            var heandler = CreateHeandler(context);
            var command = CreateCommand(savedUser, changedNote);

            // Act
            NoteDto updatedNote = await heandler.Handle(command, CancellationToken.None);

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

        // успешное без изменения категорий
        [Test]
        public async Task SuccessfullyChangingNoteWithoutChangingCategories()
        {
            // Arrange
            DataContext context = ContextManager.CreateEmptyDataContex();
            User savedUser = Helper.AddUserWithNumbers(context, 1).First();
            List<Category> savedCategories = Helper.AddCategoriesWithNumbers(context, savedUser, 1, 2);
            Note savedNote = Helper.AddNotesWithNumbers(context, savedUser, savedCategories, 1).First();

            Note changedNote = CreateChangedNote(savedNote, savedCategories);

            var heandler = CreateHeandler(context);
            var command = CreateCommand(savedUser, changedNote);

            // Act
            NoteDto updatedNote = await heandler.Handle(command, CancellationToken.None);

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
            DataContext context = ContextManager.CreateEmptyDataContex();
            User savedUser = Helper.AddUserWithNumbers(context, 1).First();
            List<Category> savedCategories = Helper.AddCategoriesWithNumbers(context, savedUser, 1, 2);
            Note savedNote = Helper.AddNotesWithNumbers(context, savedUser, savedCategories, 1).First();

            Note changedNote = CreateChangedNote(savedNote, new List<Category>());

            var heandler = CreateHeandler(context);
            var command = CreateCommand(savedUser, changedNote);

            // Act
            NoteDto updatedNote = await heandler.Handle(command, CancellationToken.None);

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
            DataContext context = ContextManager.CreateEmptyDataContex();
            User notSavedUser = Helper.CreateUserOfNumber(1);
            Note notSavedNote = Helper.CreateNoteOfNumber(1, notSavedUser, new List<Category>());

            var heandler = CreateHeandler(context);
            var command = CreateCommand(notSavedUser, notSavedNote);

            // Act / Assert
            Assert.ThrowsAsync<UserNotFoundException>(() => heandler.Handle(command, CancellationToken.None));
        }

        // заметка не найдена
        [Test]
        public void NoteNotFoundException()
        {
            // Arrange
            DataContext context = ContextManager.CreateEmptyDataContex();
            User savedUser = Helper.AddUserWithNumbers(context, 1).First();
            Note notSavedNote = Helper.CreateNoteOfNumber(1, savedUser, new List<Category>());

            var heandler = CreateHeandler(context);
            var command = CreateCommand(savedUser, notSavedNote);

            // Act / Assert
            Assert.ThrowsAsync<NoteNotFoundException>(() => heandler.Handle(command, CancellationToken.None));
        }

        // категория не найдена
        [Test]
        public void CategoryNotFoundException()
        {
            // Arrange
            DataContext context = ContextManager.CreateEmptyDataContex();
            User savedUser = Helper.AddUserWithNumbers(context, 1).First();
            Note savedNote = Helper.AddNotesWithNumbers(context, savedUser, new List<Category>(), 1).First();

            Category notSavedCategory = Helper.CreateCategoryOfNumber(1, savedUser);
            Note updatedNote = CreateChangedNote(savedNote, new List<Category>() { notSavedCategory });

            var heandler = CreateHeandler(context);
            var command = CreateCommand(savedUser, updatedNote);

            // Act / Assert
            Assert.ThrowsAsync<CategoryNotFoundException>(() => heandler.Handle(command, CancellationToken.None));

        }

        EditNoteCommandHeandler CreateHeandler(DataContext context)
        {
            var heandler = new EditNoteCommandHeandler(context, context, context, Mapper);
            return heandler;
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
