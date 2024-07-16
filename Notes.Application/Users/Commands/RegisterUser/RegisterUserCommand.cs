using MediatR;
using Notes.Application.Users.Dto;

namespace Notes.Application.Users.Commands.RegisterUser
{
    public record class RegisterUserCommand : IRequest<UserDto>
    {
        public string Login { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}
