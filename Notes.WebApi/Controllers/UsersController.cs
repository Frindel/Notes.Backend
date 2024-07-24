using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notes.Application.Users.Commands.LoginUser;
using Notes.Application.Users.Commands.RegisterUser;
using Notes.Application.Users.Commands.UpdateTokens;
using Notes.Application.Users.Dto;
using Notes.WebApi.Models.Users;

namespace Notes.WebApi.Controllers
{
    [ApiController]
    [Route("/api/users")]
    public class UsersController : BaseController
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var command = new RegisterUserCommand()
            {
                Login = request.Login,
                Password = request.Password
            };
            UserDto result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var command = new LoginUserCommand()
            {
                Login = request.Login,
                Password = request.Password
            };
            TokensDto tokens = await Mediator.Send(command);
            return Ok(tokens);
        }

        [Authorize]
        [HttpPost("update-tokens")]
        public async Task<IActionResult> UpdateTokens(UpdateTokensRequest request)
        {
            var command = new UpdateTokensCommand()
            {
                UserId = CurrentUserId,
                RefreshToken = request.RefreshToken
            };
            TokensDto newTokens = await Mediator.Send(command);
            return Ok(newTokens);
        }
    }
}
