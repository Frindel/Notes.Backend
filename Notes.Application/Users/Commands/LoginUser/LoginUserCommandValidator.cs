using FluentValidation;

namespace Notes.Application.Users.Commands.LoginUser
{
    public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidator()
        {
            RuleFor(c => c.Login).NotNull();
            RuleFor(c => c.Password).NotNull();
        }
    }
}
