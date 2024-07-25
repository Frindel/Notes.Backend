using FluentValidation;

namespace Notes.Application.Notes.Queries.GetNote;

public class GetNoteQueryValidator : AbstractValidator<GetNoteQuery>
{
    public GetNoteQueryValidator()
    {
        RuleFor(q => q.UserId).Must(ui => ui > 0).WithMessage("user is < 0");
        RuleFor(q => q.NoteId).Must(ni => ni > 0).WithMessage("note id < 0");
    }
}