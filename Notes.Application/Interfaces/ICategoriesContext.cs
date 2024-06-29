using Microsoft.EntityFrameworkCore;
using Notes.Domain;

namespace Notes.Application.Interfaces
{
    public interface ICategoriesContext
    {
        DbSet<Category> Categories { get; set; }

        Task<int> SaveChangesAsync(CancellationToken token);
    }
}
