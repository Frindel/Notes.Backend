using FluentValidation;

namespace Notes.Application.Notes.Queries.GetAllNotes;

public class GetAllNotesQueryValidator : AbstractValidator<GetAllNotesQuery>
{
    public GetAllNotesQueryValidator()
    {
        RuleFor(q => q.UserId).Must(ui => ui > 0).WithMessage("user is is null");
        RuleFor(q => q.PageNumber).Must(pn => pn > 0).WithMessage("page number < 0");
        RuleFor(q => q.PageSize).Must(ps => ps > 0).WithMessage("page size < 0");
    }
}