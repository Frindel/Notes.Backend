using MediatR;
using Notes.Application.Categories.Dto;

namespace Notes.Application.Categories.Command.Queries.GetAllCategories
{
    public sealed class GetAllCategoriesQuery : IRequest<CategoriesDto>
    {
        public int UserId { get; set; }
    }
}
