using MediatR;
using Notes.Application.Categories.Dto;

namespace Notes.Application.Categories.Command.Queries.GetCategory
{
    public sealed class GetCategoryQuery : IRequest<CategoryDto>
    {
        public int UserId { get; set; }

        public int CategoryId { get; set; }
    }
}
