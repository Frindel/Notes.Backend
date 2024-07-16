using Moq;
using Notes.Application.Common.Exceptions;
using Notes.Application.Common.Helpers;
using Notes.Application.Interfaces;
using Notes.Application.Users.Commands.UpdateTokens;
using Notes.ApplicationTests.Common;
using Notes.Domain;
using Notes.Persistence.Data;

namespace Notes.ApplicationTests.Users
{
    [TestFixture]
    internal class UpdateTokensTests : TestsBase
    {
        DataContext _context;
        Mock<IJwtTokensService> _jwtTokensServiceMock;
        UpdateTokensCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _context = ContextManager.CreateEmptyDataContex();
            _jwtTokensServiceMock = new Mock<IJwtTokensService>();
            _handler = CreateHandler();
        }

        UpdateTokensCommandHandler CreateHandler()
        {
            UsersHelper usersHelper = new UsersHelper(_context, _jwtTokensServiceMock.Object);
            return new UpdateTokensCommandHandler(usersHelper, _context, _jwtTokensServiceMock.Object);
        }

        [Test]
        public async Task SuccessfulTokenUpdate()
        {
            // Arrange
            User savedUser = Helper.AddUserWithNumbers(_context, 1).First();
            _jwtTokensServiceMock
                .Setup(tg => tg.GenerateAccessToken(It.IsAny<int>()))
                .Returns(Helper.CreateRandomStr(256));
            _jwtTokensServiceMock
                .Setup(tg => tg.GenerateRefrechToken(It.IsAny<int>()))
                .Returns(Helper.CreateRandomStr(256));
            _jwtTokensServiceMock
                .Setup(tg => tg.TokenIsValid(It.IsAny<string>()))
                .Returns(true);
            var command = CreateCommand(savedUser.Id, savedUser.RefreshToken);

            // Act
            var tokens = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(tokens);
                Assert.IsNotNull(tokens.AssessToken);
                Assert.IsNotNull(tokens.RefreshToken);
            });
        }

        [Test]
        public async Task UserNotFoundException()
        {
            // Arrange
            User notSavedUser = Helper.CreateUserOfNumber(1);
            _jwtTokensServiceMock
                .Setup(jt => jt.TokenIsValid(It.IsAny<string>()))
                .Returns(true);
            var command = CreateCommand(notSavedUser.Id, notSavedUser.RefreshToken);

            // Act / Assert
            Assert.ThrowsAsync<UserNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Test]
        public void TokenNotValidException()
        {
            // Arrange
            User savedUser = Helper.AddUserWithNumbers(_context, 1).First();
            string notValidRefreshToken = Helper.CreateRandomStr(256);
            var command = CreateCommand(savedUser.Id, notValidRefreshToken);

            // Act / Assert
            Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        }

        UpdateTokensCommand CreateCommand(int userId, string refreshToken) =>
            new UpdateTokensCommand()
            {
                UserId = userId,
                RefreshToken = refreshToken
            };
    }
}
