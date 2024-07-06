using MediatR;

namespace Notes.Application.Users.Commands.RegisterUser
{
    public class RegisterUserQuery : IRequest<UserDto>
    {
        public string Login { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}
