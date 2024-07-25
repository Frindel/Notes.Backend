using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Notes.Application.Common.Exceptions;
using Notes.Application.Users.Dto;
using Notes.Domain;
using Notes.Persistence.Data;
using Notes.Tests.Common;
using Notes.WebApi.Controllers;
using Notes.WebApi.Models.Users;

namespace Notes.Tests.WebApi
{
    [TestFixture]
    internal class UsersControllerTests : ControllerTestsBase<UsersController>
    {
        DataContext _context;
        UsersController _controller;
        User _notSavedUser;
        User _savedUser;

        [SetUp]
        public void SetUp()
        {
            _context = ContextManager.CreateEmptyDataContex();
            _controller = CreateController(_context);
            _notSavedUser = Helper.CreateUserOfNumber(1);
            _savedUser = Helper.AddUserWithNumbers(_context, 2).First();
        }

        [Test]
        public async Task RegisterUser_Success()
        {
            // Arrange
            var request = new RegisterRequest()
            {
                Login = _notSavedUser.Login,
                Password = _notSavedUser.Password
            };

            // Act
            ObjectResult response = (ObjectResult)await _controller.Register(request);
            UserDto? result = response.Value as UserDto;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(result, "result is null");
                Assert.IsNotNull(result!.AccessToken, "access-token is null");
                Assert.IsNotNull(result!.RefreshToken, "refresh-token is null");
            });
        }

        [Test]
        public void RegisterUser_MissingLogin_ThrowsValidationException()
        {
            // Arrange
            var request = new RegisterRequest()
            {
                Password = _notSavedUser.Password
            };

            // Act / Assert
            Assert.ThrowsAsync<ValidationException>(() => _controller.Register(request));
        }


        [Test]
        public void RegisterUser_MissingPassword_ThrowsValidationException()
        {
            // Arrange
            var request = new RegisterRequest()
            {
                Login = _notSavedUser.Login
            };

            // Act / Assert
            Assert.ThrowsAsync<ValidationException>(() => _controller.Register(request));
        }

        [Test]
        public async Task LoginUser_Success()
        {
            // Arrange
            var request = new LoginRequest()
            {
                Login = _savedUser.Login,
                Password = _savedUser.Password
            };

            // Act
            ObjectResult response = (ObjectResult)await _controller.Login(request);
            TokensDto? result = response.Value as TokensDto;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(result, "result is null");
                Assert.IsNotNull(result!.AssessToken, "access-token is null");
                Assert.IsNotNull(result!.RefreshToken, "refresh-token is null");
            });
        }

        [Test]
        public void LoginUser_MissingLogin_ThrowsValidationException()
        {
            // Arrange
            var request = new LoginRequest()
            {
                Password = _savedUser.Password
            };

            // Act / assert
            Assert.ThrowsAsync<ValidationException>(() => _controller.Login(request));
        }

        [Test]
        public void LoginUser_MissingPassword_ThrowsValidationException()
        {
            // Arrange
            var request = new LoginRequest()
            {
                Login = _savedUser.Login
            };

            // Act / assert
            Assert.ThrowsAsync<ValidationException>(() => _controller.Login(request));
        }

        [Test]
        public async Task UpdateTokens_Success()
        {
            // Arrange      
            Helper.SetUserIdForIdentity(_controller.HttpContext, _savedUser.Id);
            var request = new UpdateTokensRequest()
            {
                RefreshToken = _savedUser.RefreshToken
            };

            // Act
            ObjectResult response = (ObjectResult)await _controller.UpdateTokens(request);
            TokensDto? result = response.Value as TokensDto;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(result, "result is null");
                Assert.IsNotNull(result!.AssessToken, "access-token is null");
                Assert.IsNotNull(result!.RefreshToken, "refresh-token is null");
            });
        }

        [Test]
        public void UpdateTokens_MissingRefreshToken_ThrowsValidationException()
        {
            // Arrange      
            Helper.SetUserIdForIdentity(_controller.HttpContext, _savedUser.Id);
            var request = new UpdateTokensRequest();

            // Act / Assert
            Assert.ThrowsAsync<ValidationException>(() => _controller.UpdateTokens(request));
        }
    }
}
