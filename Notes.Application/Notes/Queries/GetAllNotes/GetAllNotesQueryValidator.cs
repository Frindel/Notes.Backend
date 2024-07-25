using FluentValidation;

namespace Notes.Application.Notes.Queries.GetAllNotes;

public class GetAllNotesQueryValidator : AbstractValidator<GetAllNotesQuery>
{
    public GetAllNotesQueryValidator()
    {
        RuleFor(q => q.UserId).Must(ui => ui > 0).WithMessage("user is is null");
    }
}