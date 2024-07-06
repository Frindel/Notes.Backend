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
        const string login1 = "user 1";
        const string login2 = "user 2";

        [Test]
        public void AddedUserAlreadyExistsException()
        {
            // Arrange
            DataContext context = CreateEmptyDataContex();
            context.Users.Add(new User()
            {
                Id = 1,
                Login = login1,
                Password = CreateRandomStr(10),
                RefreshToken = CreateRandomStr(256)
            });
            context.SaveChanges();

            var query = new RegisterUserQuery()
            {
                Login = login1,
                Password = CreateRandomStr(10),
            };

            var tokensGeneratorMock = new Mock<ITokensGenerator>().Object;
            var handler = new RegisterUserQueryHeandler(context, tokensGeneratorMock, Mapper);

            // Act / Assert
            Assert.ThrowsAsync<UserIsAlreadyRegisteredException>(() => handler.Handle(query, CancellationToken.None));
        }

        [Test]
        public async Task SuccessfulUserAddition()
        {
            // Arrange
            DataContext context = CreateEmptyDataContex();
            context.Users.Add(new User()
            {
                Id = 1,
                Login = login1,
                Password = CreateRandomStr(10),
                RefreshToken = CreateRandomStr(256)
            });
            context.SaveChanges();

            var query = new RegisterUserQuery()
            {
                Login = login2,
                Password = CreateRandomStr(10),
            };

            var tokensGeneratorMock = new Mock<ITokensGenerator>();
            tokensGeneratorMock
                .Setup(tg => tg.GenerateAccessToken(It.IsAny<int>()))
                .Returns(CreateRandomStr(256));
            tokensGeneratorMock
                .Setup(tg => tg.GenerateRefrechToken())
                .Returns(CreateRandomStr(256));

            var heandler = new RegisterUserQueryHeandler(context, tokensGeneratorMock.Object, Mapper);

            // Act
            var userDto = await heandler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(userDto);
                Assert.IsNotNull(userDto.Login);
                Assert.IsNotNull(userDto.AccessToken);
                Assert.IsNotNull(userDto.RefreshToken);
            });
        }

        #region Helpers

        string CreateRandomStr(int length)
        {
            string chars = "0123456789abcefghjkpqrstxyzABCEFGHJKPQRSTXYZ-_";
            StringBuilder builder = new StringBuilder();
            Random random = new Random();

            for (int index = 0; index < length; index++)
            {
                char rndChar = chars[random.Next(0, 45)];
                builder.Append(rndChar);
            }

            return builder.ToString();
        }
        #endregion
    }
}
