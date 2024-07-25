using Microsoft.EntityFrameworkCore;
using Notes.Application.Common.Exceptions;
using Notes.Application.Interfaces;
using Notes.Domain;

namespace Notes.Application.Common.Helpers
{
    public class NotesHelper : HelperBase<Note>
    {
        readonly INotesContext _notesContext;
        readonly DbSet<Note> _notes;

        public NotesHelper(INotesContext notesContext)
        {
            _notesContext = notesContext;
            _notes = _notesContext.Notes;
        }

        public Task<Note> GetNoteByIdAsync(int noteId, int forUserId) =>
             GetEntityByAsync(
                _notes.Where(c => c.PersonalId == noteId && c.User.Id == forUserId)
                    .Include(n=>n.Categories),
                typeof(NotFoundException),
                $"note with id {noteId} and user id {forUserId} does not found");

        public async Task<Note> SaveNoteAsync(Note note, CancellationToken cancellationToken)
        {
            if (note == null)
                throw new ArgumentNullException(nameof(note));
            if (note.User == null)
                throw new ArgumentNullException(nameof(note.User));

            return await SaveEntityAsync(note, _notes, () => _notesContext.SaveChangesAsync(cancellationToken));
        }
    }
}
