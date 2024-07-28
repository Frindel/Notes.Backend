namespace Notes.WebApi.Models.Categories
{
    public class CreateCategoryRequest
    {
        public string Name { get; set; } = null!;

        public string Color { get; set; } = null!;
    }
}
