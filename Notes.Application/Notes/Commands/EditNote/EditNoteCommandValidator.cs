using FluentValidation;

namespace Notes.Application.Notes.Commands.EditNote;

public class EditNoteCommandValidator : AbstractValidator<EditNoteCommand>
{
    public EditNoteCommandValidator()
    {
        RuleFor(c => c.UserId).Must(ui => ui > 0).WithMessage("user id < 0");
        RuleFor(c => c.NoteId).Must(ni => ni > 0).WithMessage("note id < 0");
        RuleFor(c => c.Name).NotNull().WithMessage("note is null");
        RuleFor(c => c.Description).NotNull().WithMessage("description is null");
        RuleFor(c => c.CategoriesIds).NotNull().WithMessage("categories ids is null");
    }
}