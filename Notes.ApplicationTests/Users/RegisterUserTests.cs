using Moq;
using Notes.Application.Common.Exceptions;
using Notes.Application.Interfaces;
using Notes.Application.Users.Commands.RegisterUser;
using Notes.ApplicationTests.Common;
using Notes.Domain;

namespace Notes.ApplicationTests.Users
{
    [TestFixture]
    internal class RegisterUserTests : TestsBase
    {

        [Test]
        public async Task AddedUserAlreadyExistsException()
        {
            // Arrange
            User firstUser = Helper.CreateUserOfNumber(1);

            IUsersContext context = ContextManager.CreateEmptyDataContex();
            context.Users.Add(firstUser);
            await context.SaveChangesAsync(CancellationToken.None);

            var command = new RegisterUserCommand()
            {
                Login = firstUser.Login,
                Password = Helper.CreateRandomStr(10),
            };

            var jwtTokensMock = new Mock<IJwtTokensService>().Object;
            var handler = new RegisterUserCommandHeandler(context, jwtTokensMock, Mapper);

            // Act / Assert
            Assert.ThrowsAsync<UserIsAlreadyRegisteredException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Test]
        public async Task SuccessfulUserAddition()
        {
            // Arrange
            User firstUser = Helper.CreateUserOfNumber(1);
            User secondUser = Helper.CreateUserOfNumber(2);

            IUsersContext context = ContextManager.CreateEmptyDataContex();
            context.Users.Add(firstUser);
            await context.SaveChangesAsync(CancellationToken.None);

            var commmand = new RegisterUserCommand()
            {
                Login = secondUser.Login,
                Password = Helper.CreateRandomStr(10),
            };

            var jwtTokensMock = new Mock<IJwtTokensService>();
            jwtTokensMock
                .Setup(tg => tg.GenerateAccessToken(It.IsAny<int>()))
                .Returns(Helper.CreateRandomStr(256));
            jwtTokensMock
                .Setup(tg => tg.GenerateRefrechToken(It.IsAny<int>()))
                .Returns(Helper.CreateRandomStr(256));

            var heandler = new RegisterUserCommandHeandler(context, jwtTokensMock.Object, Mapper);

            // Act
            var userDto = await heandler.Handle(commmand, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(userDto);
                Assert.IsNotNull(userDto.Login);
                Assert.IsNotNull(userDto.AccessToken);
                Assert.IsNotNull(userDto.RefreshToken);
            });
        }
    }
}
