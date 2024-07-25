using FluentValidation;

namespace Notes.Application.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(c => c.UserId).Must(ui => ui > 0).WithMessage("user id < 0");
            RuleFor(c => c.CategoryName).NotNull().NotEmpty().WithMessage("category name is null of empty");
        }
    }
}
