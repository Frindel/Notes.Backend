using System.Drawing;

namespace Notes.Domain
{
    public class Category
    {
        private Color _color;

        public int Id { get; set; }

        public int PersonalId { get; set; }

        public string Name { get; set; } = null!;

        #region Color

        public int ColorRgb
        {
            get => _color.ToArgb();
            set => _color = Color.FromArgb(value);
        }

        public Color Color
        {
            get => _color;
            set => _color = value;
        }

        #endregion

        public User User { get; set; } = null!;

        public List<Note> Notes { get; set; } = null!;
    }
}