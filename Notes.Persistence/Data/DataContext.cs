using Microsoft.EntityFrameworkCore;
using Notes.Application.Interfaces;
using Notes.Domain;
using Notes.Persistence.Data.DbEntitiesConfig;

namespace Notes.Persistence.Data
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
            // настройка сущностей БД
            modelBuilder.ApplyConfiguration(new UsersConfiguration());
            modelBuilder.ApplyConfiguration(new CategoriesConfiguration());
            modelBuilder.ApplyConfiguration(new NotesConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
