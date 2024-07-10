using MediatR;
using Notes.Application.Categories.Dto;

namespace Notes.Application.Categories.Command.CreateCategory
{
    public class CreateCategoryCommand : IRequest<CategoryDto>
    {
        public int UserId { get; set; }

        public string CategoryName { get; set; } = null!;
    }
}
