using Moq;
using Notes.Application.Common.Exceptions;
using Notes.Application.Interfaces;
using Notes.Application.Users.Commands.LoginUser;
using Notes.ApplicationTests.Common;
using Notes.Domain;
using NUnit.Framework.Internal;

namespace Notes.ApplicationTests.Users
{
    [TestFixture]
    internal class LoginUserTests : TestBase
    {
        [Test]
        public async Task SuccessLogin()
        {
            // Arrange
            User firstUser = Helper.CreateUserOfNumber(1);

            IUsersContext context = CreateEmptyDataContex();
            context.Users.Add(firstUser);
            await context.SaveChangesAsync(CancellationToken.None);

            var jwtTokensMock = new Mock<IJwtTokensService>();
            jwtTokensMock
                .Setup(tg => tg.GenerateAccessToken(It.IsAny<int>()))
                .Returns(Helper.CreateRandomStr(256));
            jwtTokensMock
                .Setup(tg => tg.GenerateRefrechToken(It.IsAny<int>()))
                .Returns(Helper.CreateRandomStr(256));

            var heandler = new LoginUserCommandHeandler(context, jwtTokensMock.Object);

            var command = new LoginUserCommand()
            {
                Login = firstUser.Login,
                Password = firstUser.Password
            };

            // Act
            var tokens = await heandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(tokens);
                Assert.IsNotNull(tokens.AssessToken);
                Assert.IsNotNull(tokens.RefreshToken);
            });
        }

        [Test]
        public async Task UserNotFundException()
        {
            // Arrange
            User firstUser = Helper.CreateUserOfNumber(1);
            User secondUser = Helper.CreateUserOfNumber(2);

            IUsersContext context = CreateEmptyDataContex();
            context.Users.Add(firstUser);
            await context.SaveChangesAsync(CancellationToken.None);

            var jwtTokensMock = new Mock<IJwtTokensService>();

            var heandler = new LoginUserCommandHeandler(context, jwtTokensMock.Object);

            var command = new LoginUserCommand()
            {
                Login = secondUser.Login,
                Password = secondUser.Password
            };

            // Act / Assert
            Assert.ThrowsAsync<UserNotFoundException>(() => heandler.Handle(command, CancellationToken.None));
        }

        [Test]
        public async Task InvalidPasswordException()
        {
            // Arrange
            User firstUser = Helper.CreateUserOfNumber(1);

            IUsersContext context = CreateEmptyDataContex();
            context.Users.Add(firstUser);
            await context.SaveChangesAsync(CancellationToken.None);

            var jwtTokensMock = new Mock<IJwtTokensService>();

            var heandler = new LoginUserCommandHeandler(context, jwtTokensMock.Object);

            var command = new LoginUserCommand()
            {
                Login = firstUser.Login,
                Password = Helper.CreateRandomStr(10)
            };

            // Act / Assert
            Assert.ThrowsAsync<InvalidLoginOrPasswordException>(() => heandler.Handle(command, CancellationToken.None));
        }
    }
}
