using Microsoft.AspNetCore.Mvc;
using Notes.Application.Categories.Commands.CreateCategory;
using Notes.Application.Categories.Queries.GetAllCategories;
using Notes.Application.Categories.Queries.GetCategory;
using Notes.WebApi.Models.Categories;

namespace Notes.WebApi.Controllers
{
    [ApiController]
    [Route("/api/categories")]
    public class CategoriesController : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllCategoriesQuery()
            {
                UserId = CurrentUserId
            };
            var categories = await Mediator.Send(query);
            return Ok(categories.Categories);
        }

        [HttpGet("{categoryId}")]
        public async Task<IActionResult> GetById([FromRoute] int categoryId)
        {
            var query = new GetCategoryQuery()
            {
                UserId = CurrentUserId,
                CategoryId = categoryId
            };
            var category = await Mediator.Send(query);
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
        {
            var command = new CreateCategoryCommand()
            {
                UserId = CurrentUserId,
                CategoryName = request.Name
            };
            var createdCategory = await Mediator.Send(command);
            return Ok(createdCategory);
        }
    }
}
