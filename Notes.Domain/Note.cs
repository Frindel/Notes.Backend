namespace Notes.Domain
{
    public class Note
    {
        /// <summary>
        /// Id заметки в БД
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Id заметки для пользователя
        /// </summary>
        public int PersonalId { get; set; }

        public User User { get; set; } = null!;
        
        /// <summary>
        /// Название заметки
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Описание заметки
        /// </summary>
        public string Description { get; set; } = null!;

        public DateTime Time { get; set; }


        public bool IsCompleted { get; set; }

        public List<Category> Categories { get; set; } = null!;
    }
}
