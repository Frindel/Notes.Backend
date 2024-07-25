using Moq;
using Notes.Application.Common.Exceptions;
using Notes.Application.Common.Helpers;
using Notes.Application.Interfaces;
using Notes.Application.Users.Commands.RegisterUser;
using Notes.ApplicationTests.Common;
using Notes.Domain;
using Notes.Persistence.Data;

namespace Notes.Tests.Application.Users
{
    [TestFixture]
    internal class RegisterUserTests : TestsBase
    {
        DataContext _context;
        Mock<IJwtTokensService> _jwtTokensServiceMock;
        RegisterUserCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _context = ContextManager.CreateEmptyDataContex();
            _jwtTokensServiceMock = new Mock<IJwtTokensService>();
            _handler = CreateHandler();
        }

        RegisterUserCommandHandler CreateHandler()
        {
            UsersHelper usersHelper = new UsersHelper(_context, _jwtTokensServiceMock.Object);
            return new RegisterUserCommandHandler(usersHelper, _context, Mapper);
        }

        [Test]
        public async Task RegisterUser_Success()
        {
            // Arrange
            User newUser = Helper.CreateUserOfNumber(1);
            _jwtTokensServiceMock
                .Setup(tg => tg.GenerateAccessToken(It.IsAny<int>()))
                .Returns(Helper.CreateRandomStr(256));
            _jwtTokensServiceMock
                .Setup(tg => tg.GenerateRefrechToken(It.IsAny<int>()))
                .Returns(Helper.CreateRandomStr(256));
            var command = CreateCommand(newUser.Login, newUser.Password);

            // Act
            var userDto = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(userDto);
                Assert.IsNotNull(userDto.Login);
                Assert.IsNotNull(userDto.AccessToken);
                Assert.IsNotNull(userDto.RefreshToken);
            });
        }

        [Test]
        public void RegisterUser_InvalidLogin_ThrowsUserIsAlreadyRegisteredException()
        {
            // Arrange
            User savedUser = Helper.AddUserWithNumbers(_context, 1).First();
            User notSavedUser = Helper.CreateUserOfNumber(2);
            var command = CreateCommand(savedUser.Login, notSavedUser.Password);

            // Act / Assert
            Assert.ThrowsAsync<UserIsAlreadyRegisteredException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        RegisterUserCommand CreateCommand(string login, string password) =>
            new RegisterUserCommand()
            {
                Login = login,
                Password = password
            };
    }
}