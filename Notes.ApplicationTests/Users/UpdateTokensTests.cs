using Moq;
using Notes.Application.Common.Exceptions;
using Notes.Application.Interfaces;
using Notes.Application.Users.Commands.UpdateTokens;
using Notes.ApplicationTests.Common;
using Notes.Domain;

namespace Notes.ApplicationTests.Users
{
    [TestFixture]
    internal class UpdateTokensTests : TestBase
    {
        [Test]
        public async Task SuccessfulTokenUpdate()
        {
            // Arrange
            User firstUser = Helper.CreateUserOfNumber(1);

            IUsersContext context = CreateEmptyDataContex();
            context.Users.Add(firstUser);
            await context.SaveChangesAsync(CancellationToken.None);

            var tokensGeheratorMock = new Mock<ITokensGenerator>();
            tokensGeheratorMock
                .Setup(tg => tg.GenerateAccessToken(It.IsAny<int>()))
                .Returns(Helper.CreateRandomStr(256));
            tokensGeheratorMock
                .Setup(tg => tg.GenerateRefrechToken())
                .Returns(Helper.CreateRandomStr(256));

            var heandler = new UpdateTokensCommandHeandler(context, tokensGeheratorMock.Object);

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

            IUsersContext context = CreateEmptyDataContex();
            context.Users.Add(firstUser);
            await context.SaveChangesAsync(CancellationToken.None);

            var tokensGeheratorMock = new Mock<ITokensGenerator>();
            var heandler = new UpdateTokensCommandHeandler(context, tokensGeheratorMock.Object);

            var command = new UpdateTokensCommand()
            {
                RefreshToken = secondUser.RefreshToken
            };

            // Act / Assert
            Assert.ThrowsAsync<UserNotFoundException>(() => heandler.Handle(command, CancellationToken.None));
        }
    }
}
