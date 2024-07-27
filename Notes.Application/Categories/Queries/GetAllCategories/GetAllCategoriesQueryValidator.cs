using FluentValidation;

namespace Notes.Application.Categories.Queries.GetAllCategories
{
    public class GetAllCategoriesQueryValidator : AbstractValidator<GetAllCategoriesQuery>
    {
        public GetAllCategoriesQueryValidator()
        {
            RuleFor(q => q.UserId).Must(ui => ui > 0).WithMessage("user id < 0");
            RuleFor(q => q.PageNumber).Must(pn => pn > 0).WithMessage("page number < 0");
            RuleFor(q => q.PageSize).Must(ps => ps > 0).WithMessage("page size < 0");
        }
    }
}
