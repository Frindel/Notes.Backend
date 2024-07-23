namespace Notes.WebApi.Models.Notes;

public class EditNoteRequest
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool IsCompleted { get; set; }

    public DateTime Time { get; set; }

    public List<int> CategoriesIds { get; set; } = null!;
}