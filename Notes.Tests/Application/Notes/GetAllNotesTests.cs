using Moq;
using Notes.Application.Common.Exceptions;
using Notes.Application.Common.Helpers;
using Notes.Application.Interfaces;
using Notes.Application.Notes.Queries.GetAllNotes;
using Notes.ApplicationTests.Common;
using Notes.Domain;
using Notes.Persistence.Data;

namespace Notes.Tests.Application.Notes
{
    [TestFixture]
    internal class GetAllNotesTests : TestsBase
    {
        DataContext _context;
        User _savedUser;
        List<Category> _savedCategories;
        List<Note> _savedNotes;
        GetAllNotesQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _context = ContextManager.CreateEmptyDataContex();
            _savedUser = Helper.AddUserWithNumbers(_context, 1).First();
            _savedCategories = Helper.AddCategoriesWithNumbers(_context, _savedUser, 1, 2);
            _savedNotes = Helper.AddNotesWithNumbers(_context, _savedUser, _savedCategories, 1, 2, 3);
            _handler = CreateHandler();
        }

        GetAllNotesQueryHandler CreateHandler()
        {
            var jwtTokensServiceMock = new Mock<IJwtTokensService>();
            UsersHelper usersHelper = new UsersHelper(_context, jwtTokensServiceMock.Object);

            return new GetAllNotesQueryHandler(usersHelper, _context, Mapper);
        }

        [Test]
        public async Task GetAllNotes_Success()
        {
            // Arrange
            var query = CreateQuery(_savedUser);

            // Act
            var gettedUsers = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(gettedUsers, "result is null");
                Assert.IsTrue(gettedUsers.Notes.Count == _savedNotes.Count,
                    "number of notes in the result does not match");
            });
        }

        [Test]
        public void GetAllNotes_InvalidUser_ThrowsNotFoundException()
        {
            // Arrange
            User notSavedUser = Helper.CreateUserOfNumber(2);
            var query = CreateQuery(notSavedUser);

            // Act / Assert
            Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(query, CancellationToken.None));
        }

        [Test]
        public async Task GetAllCategoriesInRange_Success()
        {
            // Arrange
            var notesIds = Enumerable.Range(3, 40).ToArray();
            Helper.AddNotesWithNumbers(_context, _savedUser, _savedCategories, notesIds);
            var query = CreateQuery(_savedUser);
            query.PageNumber = 2;

            // Act
            var notes = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(notes.Notes.Count == query.PageSize);
        }

        GetAllNotesQuery CreateQuery(User user)
        {
            return new GetAllNotesQuery()
            {
                UserId = user.Id,
                PageNumber = 1,
                PageSize = 20
            };
        }
    }
}