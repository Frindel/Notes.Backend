using FluentValidation;

namespace Notes.Application.Users.Commands.RegisterUser
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(c => c.Login).NotNull();
            RuleFor(c => c.Password).NotNull();
        }
    }
}
