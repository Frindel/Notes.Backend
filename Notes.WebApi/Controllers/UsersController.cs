using Microsoft.AspNetCore.Mvc;
using Notes.Application.Users.Commands.RegisterUser;
using Notes.Application.Users.Dto;
using Notes.WebApi.Models.Users;

namespace Notes.WebApi.Controllers
{
    [ApiController]
    [Route("/api/users")]
    public class UsersController : BaseController
    {
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(RegisterUserRequest request)
        {
            var command = new RegisterUserCommand()
            {
                Login = request.Login,
                Password = request.Password
            };
            UserDto tokens = null!;
            try
            {
                tokens = await Mediator.Send(command);
            }
            catch(Exception e)
            {

            }

            return Ok(tokens);
        }
    }
}
