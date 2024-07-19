using FluentValidation;

namespace Notes.Application.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(c => c.UserId).Must(ui => ui > 0);
            RuleFor(c => c.CategoryName).NotNull().NotEmpty();
        }
    }
}
