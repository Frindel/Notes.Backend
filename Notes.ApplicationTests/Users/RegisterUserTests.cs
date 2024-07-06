using Moq;
using Notes.Application.Common.Exceptions;
using Notes.Application.Interfaces;
using Notes.Application.Users.Commands.RegisterUser;
using Notes.ApplicationTests.Common;
using Notes.Domain;
using Notes.Persistence.Data;
using System.Text;

namespace Notes.ApplicationTests.Users
{
    [TestFixture]
    internal class RegisterUserTests : TestBase
    {

        [Test]
        public async Task AddedUserAlreadyExistsException()
        {
            // Arrange
            User firstUser = Helper.CreateUserOfNumber(1);

            IUsersContext context = CreateEmptyDataContex();
            context.Users.Add(firstUser);
            await context.SaveChangesAsync(CancellationToken.None);

            var command = new RegisterUserCommand()
            {
                Login = firstUser.Login,
                Password = Helper.CreateRandomStr(10),
            };

            var tokensGeneratorMock = new Mock<ITokensGenerator>().Object;
            var handler = new RegisterUserCommandHeandler(context, tokensGeneratorMock, Mapper);

            // Act / Assert
            Assert.ThrowsAsync<UserIsAlreadyRegisteredException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Test]
        public async Task SuccessfulUserAddition()
        {
            // Arrange
            User firstUser = Helper.CreateUserOfNumber(1);
            User secondUser = Helper.CreateUserOfNumber(2);

            IUsersContext context = CreateEmptyDataContex();
            context.Users.Add(firstUser);
            await context.SaveChangesAsync(CancellationToken.None);

            var commmand = new RegisterUserCommand()
            {
                Login = secondUser.Login,
                Password = Helper.CreateRandomStr(10),
            };

            var tokensGeneratorMock = new Mock<ITokensGenerator>();
            tokensGeneratorMock
                .Setup(tg => tg.GenerateAccessToken(It.IsAny<int>()))
                .Returns(Helper.CreateRandomStr(256));
            tokensGeneratorMock
                .Setup(tg => tg.GenerateRefrechToken())
                .Returns(Helper.CreateRandomStr(256));

            var heandler = new RegisterUserCommandHeandler(context, tokensGeneratorMock.Object, Mapper);

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
