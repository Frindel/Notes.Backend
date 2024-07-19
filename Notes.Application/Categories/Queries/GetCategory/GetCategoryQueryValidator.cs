using FluentValidation;

namespace Notes.Application.Categories.Queries.GetCategory
{
    public class GetCategoryQueryValidator : AbstractValidator<GetCategoryQuery>
    {
        public GetCategoryQueryValidator()
        {
            RuleFor(q => q.UserId).Must(ui => ui > 0);
            RuleFor(q => q.CategoryId).Must(ci => ci > 0);
        }
    }
}
