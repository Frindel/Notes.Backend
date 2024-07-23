using FluentValidation;

namespace Notes.Application.Notes.Commands.EditNote;

public class EditNoteCommandValidator : AbstractValidator<EditNoteCommand>
{
    public EditNoteCommandValidator()
    {
        RuleFor(c => c.UserId).Must(ui => ui > 0);
        RuleFor(c => c.NoteId).Must(ni => ni > 0);
        RuleFor(c => c.Name).NotNull();
        RuleFor(c => c.Description).NotNull();
        RuleFor(c => c.CategoriesIds).NotNull();
    }
}