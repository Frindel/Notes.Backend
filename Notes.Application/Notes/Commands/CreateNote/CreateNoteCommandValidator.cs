using FluentValidation;

namespace Notes.Application.Notes.Commands.CreateNote;

public class CreateNoteCommandValidator : AbstractValidator<CreateNoteCommand>
{
    public CreateNoteCommandValidator()
    {
        RuleFor(c => c.UserId).Must(ui => ui > 0).WithMessage("user id < 0");
        RuleFor(c => c.Name).NotNull().WithMessage("name is null");
        RuleFor(c => c.Description).NotNull().WithMessage("description is null");
        RuleFor(c => c.CategoriesIds).NotNull().WithMessage("categories ids is null");
    }
}