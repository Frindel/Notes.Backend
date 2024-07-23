using FluentValidation;

namespace Notes.Application.Notes.Commands.CreateNote;

public class CreateNoteCommandValidator : AbstractValidator<CreateNoteCommand>
{
    public CreateNoteCommandValidator()
    {
        RuleFor(c => c.UserId).Must(ui => ui > 0);
        RuleFor(c => c.Name).NotNull();
        RuleFor(c => c.Description).NotNull();
        RuleFor(c => c.CategoriesIds).NotNull();
    }
}