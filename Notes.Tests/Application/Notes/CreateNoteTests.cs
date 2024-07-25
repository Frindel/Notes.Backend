using Moq;
using Notes.Application.Common.Exceptions;
using Notes.Application.Common.Helpers;
using Notes.Application.Interfaces;
using Notes.Application.Notes.Commands.CreateNote;
using Notes.ApplicationTests.Common;
using Notes.Domain;
using Notes.Persistence.Data;

namespace Notes.Tests.Application.Notes
{
    [TestFixture]
    internal class CreateNoteTests : TestsBase
    {
        DataContext _context;
        User _savedUser;
        List<Category> _savedCategories;
        CreateNoteCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _context = ContextManager.CreateEmptyDataContex();
            _savedUser = Helper.AddUserWithNumbers(_context, 1).First();
            _savedCategories = Helper.AddCategoriesWithNumbers(_context, _savedUser, 1, 2);
            _handler = CreateHandler();
        }

        CreateNoteCommandHandler CreateHandler()
        {
            var jwtTokensServiceMock = new Mock<IJwtTokensService>();
            UsersHelper usersHelper = new UsersHelper(_context, jwtTokensServiceMock.Object);

            CategoriesHelper categoriesHelper = new CategoriesHelper(_context);
            NotesHelper notesHelper = new NotesHelper(_context);

            var handler = new CreateNoteCommandHandler(usersHelper, categoriesHelper, notesHelper, _context, Mapper);
            return handler;
        }

        [Test]
        public async Task CreateNote_Success()
        {
            // Arrange
            var command = CreateCommand(_savedUser, _savedCategories);

            // Act
            var createdNote = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(createdNote, "result is null");
                Assert.IsNotNull(createdNote.Name, "created note name is not sated");
                Assert.IsNotNull(createdNote.Description, "created note description is not sated");
                Assert.IsTrue(createdNote.Categories.Count() == command.CategoriesIds.Count(),
                    "sated categories is not saved");
            });
        }

        [Test]
        public void CreateNote_InvalidUser_ThrowsNotFoundException()
        {
            // Arrange
            User notSavedUser = Helper.CreateUserOfNumber(2);
            var command = CreateCommand(notSavedUser);

            // Act / assert
            Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Test]
        public void CreateNote_InvalidCategory_ThrowsNotFoundException()
        {
            // Arrange
            Category notSavedCategory = Helper.CreateCategoryOfNumber(3, _savedUser);
            var command = CreateCommand(_savedUser, [notSavedCategory]);

            // Act / assert
            Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
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