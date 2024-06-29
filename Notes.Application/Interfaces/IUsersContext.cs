using Microsoft.EntityFrameworkCore;
using Notes.Domain;

namespace Notes.Application.Interfaces
{
    public interface IUsersContext
    {
        DbSet<User> Users { get; set; }

        Task<int> OnSaveChangesAsync(CancellationToken token);
    }
}
