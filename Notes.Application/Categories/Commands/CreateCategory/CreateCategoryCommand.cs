using MediatR;
using Notes.Application.Categories.Dto;

namespace Notes.Application.Categories.Commands.CreateCategory
{
    public record class CreateCategoryCommand : IRequest<CategoryDto>
    {
        public int UserId { get; set; }

        public string CategoryName { get; set; } = null!;
    }
}
