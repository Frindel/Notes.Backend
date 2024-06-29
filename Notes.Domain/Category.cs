namespace Notes.Domain
{
    public class Category
    {
        public int Id { get; set; }

        public int PersonalId { get; set; }

        public string Name { get; set; } = null!;

        public User User { get; set; } = null!;

        public List<Note> Notes { get; set; } = null!;
    }
}
