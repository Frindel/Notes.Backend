using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Notes.Application.Common.Exceptions;
using Notes.Application.Notes.Dto;
using Notes.Domain;
using Notes.Persistence.Data;
using Notes.Tests.Common;
using Notes.WebApi.Controllers;
using Notes.WebApi.Models.Notes;

namespace Notes.Tests.WebApi
{
    [TestFixture]
    internal class NotesControllerTests : ControllerTestsBase<NotesController>
    {
        private DataContext _context;
        private NotesController _controller;
        private User _notSavedUser;
        private User _savedUser;

        [SetUp]
        public void SetUp()
        {
            _context = ContextManager.CreateEmptyDataContex();
            _controller = CreateController(_context);
            _savedUser = Helper.AddUserWithNumbers(_context, 1).First();
            _notSavedUser = Helper.CreateUserOfNumber(2);
        }

        #region GetAllNotes

        [Test]
        public async Task GetAllNotes_Success()
        {
            // Arrange
            Helper.SetUserIdForIdentity(_controller.HttpContext, _savedUser.Id);
            Category savedCategory = Helper.AddCategoriesWithNumbers(_context, _savedUser, 1).First();
            List<Note> savedNotes =
                Helper.AddNotesWithNumbers(_context, _savedUser, new List<Category>() { savedCategory }, 1, 2);

            // Act
            ObjectResult response = (ObjectResult)await _controller.GetAll();
            List<NoteDto>? notes = response.Value as List<NoteDto>;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(notes, "value is null");
                Assert.IsTrue(notes!.Count == savedNotes.Count, "notes not geted");
                Assert.IsNotNull(notes[0].Categories, "categories list is null");
                Assert.IsNotEmpty(notes[0].Categories, "categories list is empty");
            });
        }

        [Test]
        public void GetAllNotes_NegativeUserId_ThrowsValudationException()
        {
            // Arrange
            Helper.SetUserIdForIdentity(_controller.HttpContext, -1);

            // Act / assert
            Assert.ThrowsAsync<ValidationException>(() => _controller.GetAll());
        }

        #endregion

        #region GetNoteById

        [Test]
        public async Task GetNoteById_Success()
        {
            // Arrange
            Helper.SetUserIdForIdentity(_controller.HttpContext, _savedUser.Id);
            Category savedCategory = Helper.AddCategoriesWithNumbers(_context, _savedUser, 1).First();
            Note savedNote = Helper.AddNotesWithNumbers(_context, _savedUser, new List<Category>() { savedCategory }, 1)
                .First();

            // Act
            ObjectResult response = (ObjectResult)await _controller.GetById(savedNote.PersonalId);
            NoteDto? note = response.Value as NoteDto;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(note, "result is null");
                Assert.IsNotNull(note!.Name, "note name is null");
                Assert.IsNotNull(note!.Description, "note name is null");
                Assert.IsNotNull(note.Categories, "categories list is null");
                Assert.IsNotEmpty(note.Categories, "categories is empty");
            });
        }

        [Test]
        public void GetNoteById_NegativeUserIdOrNegativeNoteId_ThrowsValidationException()
        {
            const int negativeValue = -1;

            // Arrange
            Helper.SetUserIdForIdentity(_controller.HttpContext, -1);

            // Act / Assert
            Assert.ThrowsAsync<ValidationException>(() => _controller.GetById(-1));
        }

        [Test]
        public void GetNoteById_NonExistentUser_ThrowsUserNotFoundException()
        {
            // Arrange
            Helper.SetUserIdForIdentity(_controller.HttpContext, _notSavedUser.Id);
            Note notSavedNote = Helper.CreateNoteOfNumber(1, _notSavedUser);

            // Act / Assert
            Assert.ThrowsAsync<UserNotFoundException>(() => _controller.GetById(notSavedNote.PersonalId));
        }

        [Test]
        public void GetNoteById_NonExistentNote_ThrowsNoteNotFoundException()
        {
            // Arrange
            Helper.SetUserIdForIdentity(_controller.HttpContext, _savedUser.Id);
            Note notSavedNote = Helper.CreateNoteOfNumber(1, _savedUser);

            // Act / Assert
            Assert.ThrowsAsync<NoteNotFoundException>(() => _controller.GetById(notSavedNote.PersonalId));
        }

        #endregion

        #region CreateNote

        [Test]
        public async Task CreateNote_Success()
        {
            // Arrange
            Helper.SetUserIdForIdentity(_controller.HttpContext, _savedUser.Id);
            List<Category> savedCategories = Helper.AddCategoriesWithNumbers(_context, _savedUser, 1, 2);
            Note addedNote = Helper.CreateNoteOfNumber(1, _savedUser, savedCategories);
            var request = CreateAddingRequest(addedNote);

            // Act
            ObjectResult response = (ObjectResult)await _controller.Create(request);
            NoteDto? createdNote = response.Value as NoteDto;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(createdNote, "result is null");
                Assert.IsNotNull(createdNote!.Name, "note name is null");
                Assert.IsNotNull(createdNote!.Description, "note description is null");
                Assert.IsNotNull(createdNote.Categories, "categories list is null");
                Assert.IsNotEmpty(createdNote.Categories, "categories list is empty");
            });
        }

        [Test]
        public void CreateNote_NegativeUserId_ThrowsValidationException()
        {
            const int negativeValue = -1;

            // Arrange
            Helper.SetUserIdForIdentity(_controller.HttpContext, negativeValue);
            Note addedNote = Helper.CreateNoteOfNumber(1, _savedUser);
            var request = CreateAddingRequest(addedNote);

            // Act / Assert
            Assert.ThrowsAsync<ValidationException>(() => _controller.Create(request));
        }

        [Test]
        public void CreateNote_NullValuesInRequest_ThrowsValidationException()
        {
            // Arrange
            Helper.SetUserIdForIdentity(_controller.HttpContext, _savedUser.Id);
            Note addedNote = Helper.CreateNoteOfNumber(1, _savedUser);
            addedNote.Name = addedNote.Description = null!;
            addedNote.Categories = null!;
            var request = CreateAddingRequest(addedNote);

            // Act / assert
            Assert.ThrowsAsync<ValidationException>(() => _controller.Create(request));
        }

        [Test]
        public void CreateNote_NonExistentUser_ThrowsUserNotFoundException()
        {
            // Arrange
            Helper.SetUserIdForIdentity(_controller.HttpContext, _notSavedUser.Id);
            Note addedNote = Helper.CreateNoteOfNumber(1, _notSavedUser);
            var request = CreateAddingRequest(addedNote);

            // Act / assert
            Assert.ThrowsAsync<UserNotFoundException>(() => _controller.Create(request));
        }

        [Test]
        public void CreateNote_NonExistentCategory_ThrowsCategoryNotFoundException()
        {
            // Arrange
            Helper.SetUserIdForIdentity(_controller.HttpContext, _savedUser.Id);
            Category notSavedCategory = Helper.CreateCategoryOfNumber(1, _notSavedUser);
            Note addedNote = Helper.CreateNoteOfNumber(1, _savedUser, new List<Category>() { notSavedCategory });
            var request = CreateAddingRequest(addedNote);

            // Act / assert
            Assert.ThrowsAsync<CategoryNotFoundException>(() => _controller.Create(request));
        }

        CreateNoteRequest CreateAddingRequest(Note addedNote) =>
            new CreateNoteRequest()
            {
                Name = addedNote.Name,
                Description = addedNote.Description,
                Time = addedNote.Time,
                CategoriesIds = GetCategoriesId(addedNote.Categories)
            };

        List<int> GetCategoriesId(List<Category> categories)
        {
            if (categories == null)
                return new List<int>();
            return categories.Select(c => c.PersonalId).ToList();
        }

        #endregion

        #region EditNote

        [Test]
        public async Task EditNote_Success()
        {
            // Arrange
            Helper.SetUserIdForIdentity(_controller.HttpContext, _savedUser.Id);
            List<Category> savedCategories = Helper.AddCategoriesWithNumbers(_context, _savedUser, 1, 2);
            var editableNote = Helper.AddNotesWithNumbers(_context, _savedUser, savedCategories, 1).First();

            var request = CreateChangeRequest(editableNote);

            // Act
            ObjectResult response = (ObjectResult)await _controller.Edit(request);
            NoteDto? changedNote = response.Value as NoteDto;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(changedNote, "value is null");
                Assert.AreEqual(request!.Name, changedNote!.Name);
                Assert.AreEqual(request!.Description, changedNote!.Description);
                Assert.AreEqual(request!.Time, changedNote.Time);
                Assert.AreEqual(request!.IsCompleted, changedNote.IsCompleted);
                Assert.IsNotNull(changedNote.Categories);
                Assert.IsTrue(changedNote.Categories.Count == editableNote.Categories.Count);
            });
        }

        [Test]
        public void EditNote_NegativeUserId_ThrowsValidationException()
        {
            // Arrange
            Helper.SetUserIdForIdentity(_controller.HttpContext, -1);
            List<Category> savedCategories = Helper.AddCategoriesWithNumbers(_context, _savedUser, 1, 2);
            var editableNote = Helper.AddNotesWithNumbers(_context, _savedUser, savedCategories, 1).First();
            var request = CreateChangeRequest(editableNote);

            // Act / assert
            Assert.ThrowsAsync<ValidationException>(() => _controller.Edit(request));
        }

        [Test]
        public void EditNote_NegativeNoteId_ThrowsValidationException()
        {
            // Arrange
            Helper.SetUserIdForIdentity(_controller.HttpContext, _savedUser.Id);
            List<Category> savedCategories = Helper.AddCategoriesWithNumbers(_context, _savedUser, 1, 2);
            var editableNote = Helper.CreateNoteOfNumber(-1, _savedUser);
            var request = CreateChangeRequest(editableNote);

            // Act / assert
            Assert.ThrowsAsync<ValidationException>(() => _controller.Edit(request));
        }

        [Test]
        public void EditNote_NullValuesInRequest_ThrowsValidationException()
        {
            // Arrange
            Helper.SetUserIdForIdentity(_controller.HttpContext, _savedUser.Id);
            List<Category> savedCategories = Helper.AddCategoriesWithNumbers(_context, _savedUser, 1, 2);
            var editableNote = Helper.CreateNoteOfNumber(-1, _savedUser);
            editableNote.Name = editableNote.Description = null!;
            editableNote.Categories = null!;
            var request = CreateChangeRequest(editableNote);

            // Act / assert
            Assert.ThrowsAsync<ValidationException>(() => _controller.Edit(request));
        }

        [Test]
        public void EditNote_NonExistentUser_ThrowsUserNotFoundException()
        {
            // Arrange
            Helper.SetUserIdForIdentity(_controller.HttpContext, _notSavedUser.Id);
            var editableNote = Helper.CreateNoteOfNumber(-1, _savedUser);
            editableNote.Name = editableNote.Description = null!;
            editableNote.Categories = null!;
            var request = CreateChangeRequest(editableNote);

            // Act / assert
            Assert.ThrowsAsync<ValidationException>(() => _controller.Edit(request));
        }

        [Test]
        public void EditNote_NonExistentNote_ThrowsUserNotFoundException()
        {
            // Arrange
            Helper.SetUserIdForIdentity(_controller.HttpContext, _savedUser.Id);
            var editableNote = Helper.CreateNoteOfNumber(1, _savedUser);
            var request = CreateChangeRequest(editableNote);

            // Act / assert
            Assert.ThrowsAsync<NoteNotFoundException>(() => _controller.Edit(request));
        }

        [Test]
        public void EditNote_NonExistentCategory_ThrowsCategoryNotFoundException()
        {
            // Arrange
            Helper.SetUserIdForIdentity(_controller.HttpContext, _savedUser.Id);
            var notSavedCategory = Helper.CreateCategoryOfNumber(1, _savedUser);
            var editableNote = Helper.AddNotesWithNumbers(_context, _savedUser, null, 1).First();
            var request = CreateChangeRequest(editableNote);
            request.CategoriesIds = [notSavedCategory.Id];

            // Act / assert
            Assert.ThrowsAsync<CategoryNotFoundException>(() => _controller.Edit(request));
        }

        EditNoteRequest CreateChangeRequest(Note editableNote) =>
            new EditNoteRequest()
            {
                Id = editableNote.PersonalId,
                Name = $"changed {editableNote.Name}",
                Description = $"changed {editableNote.Description}",
                Time = editableNote.Time.AddDays(1),
                IsCompleted = editableNote.IsCompleted!,
                CategoriesIds = GetCategoriesId(editableNote.Categories)
            };

        #endregion

        #region DeleteNote

        [Test]
        public async Task DeleteNote_Success()
        {
            // Arrange
            Helper.SetUserIdForIdentity(_controller.HttpContext, _savedUser.Id);
            List<Category> savedCategories = Helper.AddCategoriesWithNumbers(_context, _savedUser, 1, 2);
            Note savedNote = Helper.AddNotesWithNumbers(_context, _savedUser, savedCategories, 1).First();

            // Act
            StatusCodeResult response = (StatusCodeResult)await _controller.Delete(savedNote.Id);

            // Assert
            Assert.IsTrue(response.StatusCode == StatusCodes.Status204NoContent);
        }

        [Test]
        public void DeleteNote_NegativeUserId_ThrowsValidateException()
        {
            const int negativeValue = -1;

            // Arrange
            Helper.SetUserIdForIdentity(_controller.HttpContext, negativeValue);
            Note deletedNote = Helper.CreateNoteOfNumber(1, _notSavedUser);

            // Act / assert
            Assert.ThrowsAsync<ValidationException>(() => _controller.Delete(deletedNote.PersonalId));
        }

        [Test]
        public void DeleteNote_NegativeNoteId_ThrowsValidateException()
        {
            const int negativeValue = -1;

            // Arrange
            Helper.SetUserIdForIdentity(_controller.HttpContext, _savedUser.Id);
            Note deletedNote = Helper.CreateNoteOfNumber(negativeValue, _savedUser);

            // Act / assert
            Assert.ThrowsAsync<ValidationException>(() => _controller.Delete(deletedNote.PersonalId));
        }

        [Test]
        public void DeleteNote_NonExistentUser_ThrowsUsersNotFoundException()
        {
            // Arrange
            Helper.SetUserIdForIdentity(_controller.HttpContext, _notSavedUser.Id);
            Note deletedNote = Helper.AddNotesWithNumbers(_context, _savedUser, null!, 1).First();

            // Act / assert
            Assert.ThrowsAsync<UserNotFoundException>(() => _controller.Delete(deletedNote.PersonalId));
        }

        [Test]
        public void DeleteNote_NonExistentNote_ThrowsNoteNoteFoundException()
        {
            // Arrange
            Helper.SetUserIdForIdentity(_controller.HttpContext, _savedUser.Id);
            Note deletedNote = Helper.CreateNoteOfNumber(1, _savedUser);

            // Act / assert
            Assert.ThrowsAsync<NoteNotFoundException>(() => _controller.Delete(deletedNote.PersonalId));
        }

        #endregion
    }
}