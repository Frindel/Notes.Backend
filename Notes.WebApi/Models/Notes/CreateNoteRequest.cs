namespace Notes.WebApi.Models.Notes;

public class CreateNoteRequest
{
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime Time { get; set; }

    public List<int> CategoriesIds { get; set; } = null!;
}