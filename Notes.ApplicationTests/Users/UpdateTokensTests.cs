using Moq;
using Notes.Application.Common.Exceptions;
using Notes.Application.Interfaces;
using Notes.Application.Users.Commands.UpdateTokens;
using Notes.ApplicationTests.Common;
using Notes.Domain;

namespace Notes.ApplicationTests.Users
{
    [TestFixture]
    internal class UpdateTokensTests : TestsBase
    {
        [Test]
        public async Task SuccessfulTokenUpdate()
        {
            // Arrange
            User firstUser = Helper.CreateUserOfNumber(1);

            IUsersContext context = ContextManager.CreateEmptyDataContex();
            context.Users.Add(firstUser);
            await context.SaveChangesAsync(CancellationToken.None);

            var jwtTokensMock = new Mock<IJwtTokensService>();
            jwtTokensMock
                .Setup(tg => tg.GenerateAccessToken(It.IsAny<int>()))
                .Returns(Helper.CreateRandomStr(256));
            jwtTokensMock
                .Setup(tg => tg.GenerateRefrechToken(It.IsAny<int>()))
                .Returns(Helper.CreateRandomStr(256));
            jwtTokensMock
                .Setup(tg => tg.TokenIsValid(It.IsAny<string>()))
                .Returns(true);

            var heandler = new UpdateTokensCommandHeandler(context, jwtTokensMock.Object);

            var command = new UpdateTokensCommand()
            {
                RefreshToken = firstUser.RefreshToken
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
        public async Task UserNotFoundException()
        {
            // Arrange
            User firstUser = Helper.CreateUserOfNumber(1);
            User secondUser = Helper.CreateUserOfNumber(2);

            IUsersContext context = ContextManager.CreateEmptyDataContex();
            context.Users.Add(firstUser);
            await context.SaveChangesAsync(CancellationToken.None);

            var jwtTokensMock = new Mock<IJwtTokensService>();
            jwtTokensMock
                .Setup(jt => jt.TokenIsValid(It.IsAny<string>()))
                .Returns(true);

            var heandler = new UpdateTokensCommandHeandler(context, jwtTokensMock.Object);
            var command = new UpdateTokensCommand()
            {
                RefreshToken = secondUser.RefreshToken
            };

            // Act / Assert
            Assert.ThrowsAsync<UserNotFoundException>(() => heandler.Handle(command, CancellationToken.None));
        }

        [Test]
        public void TokenNotValidException()
        {
            // Arrange
            IUsersContext context = ContextManager.CreateEmptyDataContex();

            var jwtTokensMock = new Mock<IJwtTokensService>();
            jwtTokensMock
                .Setup(jt => jt.TokenIsValid(It.IsAny<string>()))
                .Returns(false);

            var heandler = new UpdateTokensCommandHeandler(context, jwtTokensMock.Object);
            var command = new UpdateTokensCommand()
            {
                RefreshToken = Helper.CreateRandomStr(256)
            };


            // Act / Assert
            Assert.ThrowsAsync<ValidationException>(() => heandler.Handle(command, CancellationToken.None));
        }
    }
}
