using FluentValidation;

namespace Notes.Application.Users.Commands.RegisterUser
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(c => c.Login).NotNull().WithMessage("login is null");
            RuleFor(c => c.Password).NotNull().WithMessage("password is null");
        }
    }
}
