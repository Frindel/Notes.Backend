using Microsoft.EntityFrameworkCore;
using Notes.Domain;

namespace Notes.Application.Interfaces
{
    public interface INotesContext
    {
        DbSet<Note> Notes { get; set; }

        Task<int> SaveChanges(CancellationToken token);
    }
}
