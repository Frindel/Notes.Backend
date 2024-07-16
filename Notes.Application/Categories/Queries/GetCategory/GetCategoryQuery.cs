using MediatR;
using Notes.Application.Categories.Dto;

namespace Notes.Application.Categories.Queries.GetCategory
{
    public record class GetCategoryQuery : IRequest<CategoryDto>
    {
        public int UserId { get; set; }

        public int CategoryId { get; set; }
    }
}
