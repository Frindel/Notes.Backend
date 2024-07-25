using Notes.ApplicationTests.Common;
using Notes.Domain;
using Notes.Persistence;

namespace Notes.Tests.Persistence
{
    [TestFixture]
    internal class JwtTokensServiceTests : TestsBase
    {
        const string secret = "fq2p089bsd-few22-4aw230923rasjdf_wfwo317098";
        const string issuer = "notes_issuer";
        const string audience = "notes_audience";

        User _targetUser;

        [SetUp]
        public void SetUp()
        {
            _targetUser = Helper.CreateUserOfNumber(1);
        }

        [Test]
        public void GenerateTokens_Success()
        {
            // Arange
            int accessTokenLive = 1000;
            int refreshTokenLive = 1000;
            var jwtService = CreateService(accessTokenLive, refreshTokenLive);

            // Act
            string accessToken = jwtService.GenerateAccessToken(_targetUser.Id);
            string refreshToken = jwtService.GenerateRefrechToken(_targetUser.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(accessToken, "access-token is null");
                Assert.IsNotNull(refreshToken, "refresh-token is null");
            });
        }

        [Test]
        public void GenerateAccessToken_ExpiredTokenIsInvalid()
        {
            // Arrange
            int accessTokenLive = 1;
            int refreshTokenLive = 1;
            var jwtService = CreateService(accessTokenLive, refreshTokenLive);

            // Act
            string accessToken = jwtService.GenerateAccessToken(_targetUser.Id);
            Thread.Sleep(2000);
            bool tokenIsValid = jwtService.TokenIsValid(accessToken);

            // Accert
            Assert.IsFalse(tokenIsValid, "incorrect validity of the token");
        }

        [Test]
        public void GenerateAccessToken_TokenExpiresAfterSpecifiedTime()
        {
            // Arrange
            int accessTokenLive = 1;
            int refreshTokenLive = 1;
            var jwtService = CreateService(accessTokenLive, refreshTokenLive);

            // Act
            string accessToken = jwtService.GenerateAccessToken(_targetUser.Id);
            Thread.Sleep(2000);
            bool tokenIsValid = jwtService.TokenIsValid(accessToken);

            // Assert
            Assert.IsFalse(tokenIsValid);
        }


        JwtTokensService CreateService(int accessTokenLiveTime, int refreshTokenLiveTime) =>
            new JwtTokensService(secret, issuer, audience, accessTokenLiveTime, refreshTokenLiveTime);

    }
}
