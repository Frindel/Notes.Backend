using Moq;
using Notes.Application.Common.Exceptions;
using Notes.Application.Common.Helpers;
using Notes.Application.Interfaces;
using Notes.Application.Users.Commands.LoginUser;
using Notes.ApplicationTests.Common;
using Notes.Domain;
using Notes.Persistence.Data;
using NUnit.Framework.Internal;

namespace Notes.Tests.Application.Users
{
    [TestFixture]
    internal class LoginUserTests : TestsBase
    {
        DataContext _context;
        Mock<IJwtTokensService> _jwtTokensServiceMock;
        LoginUserCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _context = ContextManager.CreateEmptyDataContex();
            _jwtTokensServiceMock = new Mock<IJwtTokensService>();
            _handler = CreateHandler();
        }

        LoginUserCommandHandler CreateHandler()
        {
            UsersHelper usersHelper = new UsersHelper(_context, _jwtTokensServiceMock.Object);
            return new LoginUserCommandHandler(usersHelper);
        }

        [Test]
        public async Task SuccessLogin()
        {
            // Arrange
            User savedUser = Helper.AddUserWithNumbers(_context, 1).First();
            _jwtTokensServiceMock
                .Setup(tg => tg.GenerateAccessToken(It.IsAny<int>()))
                .Returns(Helper.CreateRandomStr(256));
            _jwtTokensServiceMock
                .Setup(tg => tg.GenerateRefrechToken(It.IsAny<int>()))
                .Returns(Helper.CreateRandomStr(256));
            var command = CreateCommand(savedUser.Login, savedUser.Password);

            // Act
            var getedTokens = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(getedTokens);
                Assert.IsNotNull(getedTokens.AssessToken);
                Assert.IsNotNull(getedTokens.RefreshToken);
            });
        }

        [Test]
        public async Task UserNotFundException()
        {
            // Arrange
            User notSavedUser = Helper.CreateUserOfNumber(1);
            var command = CreateCommand(notSavedUser.Login, notSavedUser.Password);

            // Act / Assert
            Assert.ThrowsAsync<UserNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Test]
        public async Task InvalidPasswordException()
        {
            // Arrange
            User savedUser = Helper.AddUserWithNumbers(_context, 1).First();
            User notSavedUser = Helper.CreateUserOfNumber(2);
            var command = CreateCommand(savedUser.Login, notSavedUser.Password);

            // Act / Assert
            Assert.ThrowsAsync<InvalidLoginOrPasswordException>(() => _handler.Handle(command, CancellationToken.None));
        }

        LoginUserCommand CreateCommand(string login, string password) =>
            new LoginUserCommand()
            {
                Login = login,
                Password = password
            };

    }
}
