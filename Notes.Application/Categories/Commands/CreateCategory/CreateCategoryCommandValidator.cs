using FluentValidation;

namespace Notes.Application.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(c => c.UserId).Must(ui => ui > 0).WithMessage("user id < 0");
            RuleFor(c => c.CategoryName).NotNull().NotEmpty().WithMessage("category name is null of empty");
            RuleFor(c => c.Color).NotNull().Matches("^#(?:[0-9a-fA-F]{3}){1,2}$").WithMessage("incorrect color format");
        }
    }
}