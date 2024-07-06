using MediatR;

namespace Notes.Application.Users.Commands.RegisterUser
{
    public class RegisterUserCommand : IRequest<UserDto>
    {
        public string Login { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}
