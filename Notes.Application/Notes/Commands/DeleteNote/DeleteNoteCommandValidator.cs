using FluentValidation;

namespace Notes.Application.Notes.Commands.DeleteNote;

public class DeleteNoteCommandValidator : AbstractValidator<DeleteNoteCommand>
{
    public DeleteNoteCommandValidator()
    {
        RuleFor(c => c.UserId).Must(ui => ui > 0);
        RuleFor(c => c.NoteId).Must(ni => ni > 0);
    }
}