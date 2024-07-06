using MediatR;
using Notes.Application.Users.Dto;

namespace Notes.Application.Users.Commands.LoginUser
{
    public record class LoginUserCommand : IRequest<TokensDto>
    {
        public string Login { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}
