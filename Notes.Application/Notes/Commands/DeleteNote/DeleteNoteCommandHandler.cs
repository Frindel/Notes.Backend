using MediatR;
using Notes.Application.Common.Helpers;
using Notes.Application.Interfaces;
using Notes.Domain;

namespace Notes.Application.Notes.Commands.DeleteNote
{
    public class DeleteNoteCommandHandler : IRequestHandler<DeleteNoteCommand>
    {
        readonly UsersHelper _usersHelper;
        readonly NotesHelper _notesHelper;
        readonly INotesContext _notesContext;

        public DeleteNoteCommandHandler(UsersHelper usersHelper, NotesHelper notesHelper,
            INotesContext notesContex)
        {
            _usersHelper = usersHelper;
            _notesHelper = notesHelper;
            _notesContext = notesContex;
        }
        public async Task Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
        {
            User user = await _usersHelper.GetUserByIdAsync(request.UserId);
            Note deletedNote = await _notesHelper.GetNoteByIdAsync(request.NoteId, user.Id);
            await DeleteNoteAsync(deletedNote, cancellationToken);
        }

        async Task DeleteNoteAsync(Note deletedNote, CancellationToken cancellationToken)
        {
            _notesContext.Notes.Remove(deletedNote);
            await _notesContext.SaveChangesAsync(cancellationToken);
        }
    }
}
