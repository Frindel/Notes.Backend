using Microsoft.EntityFrameworkCore;
using Notes.Application.Interfaces;
using Notes.Domain;

namespace Notes.Persistence
{
    public class DataContext : DbContext, IUsersContext, ICategoriesContext, INotesContext
    {
        public DbSet<User> Users { get; set; } = null!;

        public DbSet<Category> Categories { get; set; } = null!;
        
        public DbSet<Note> Notes { get; set; } = null!;

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // todo: настройка сущностей БД

            base.OnModelCreating(modelBuilder);
        }
    }
}
