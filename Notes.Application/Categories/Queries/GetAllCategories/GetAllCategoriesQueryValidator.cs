using FluentValidation;

namespace Notes.Application.Categories.Queries.GetAllCategories
{
    public class GetAllCategoriesQueryValidator : AbstractValidator<GetAllCategoriesQuery>
    {
        public GetAllCategoriesQueryValidator()
        {
            RuleFor(q => q.UserId).Must(ui => ui > 0).WithMessage("user id < 0");
        }
    }
}
