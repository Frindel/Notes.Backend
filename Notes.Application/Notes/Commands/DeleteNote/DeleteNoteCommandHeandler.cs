using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Notes.Application.Common.Exceptions;
using Notes.Application.Interfaces;
using Notes.Domain;

namespace Notes.Application.Notes.Commands.DeleteNote
{
    public class DeleteNoteCommandHeandler : IRequestHandler<DeleteNoteCommand>
    {
        INotesContext _notesContext;
        IUsersContext _usersContext;
        IMapper _mapper;

        public DeleteNoteCommandHeandler(INotesContext notesContex, IUsersContext usersContext, IMapper mapper)
        {
            _notesContext = notesContex;
            _usersContext = usersContext;
            _mapper = mapper;
        }
        public async Task Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
        {
            User user = await GetUserByIdAsync(request.UserId, cancellationToken);
            Note deletedNote = await GetNoteByIdAsync(request.NoteId, user, cancellationToken);

            await DeleteNoteAsync(deletedNote, cancellationToken);
        }

        async Task<User> GetUserByIdAsync(int userId, CancellationToken cancellationToken)
        {
            User? selectedUser = await _usersContext.Users
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (selectedUser == null!)
                throw new UserNotFoundException("User not found");

            return selectedUser;
        }

        async Task<Note> GetNoteByIdAsync(int noteId, User targetUser, CancellationToken cancellationToken)
        {
            Note? selectedNote = await _notesContext.Notes
                .FirstOrDefaultAsync(n => n.PersonalId == noteId && n.User.Id == targetUser.Id, cancellationToken);
            if (selectedNote == null)
                throw new NoteNotFoundException($"Note with id = {noteId} not found");

            return selectedNote;
        }

        async Task DeleteNoteAsync(Note deletedNote, CancellationToken cancellationToken)
        {
            _notesContext.Notes.Remove(deletedNote);
            await _notesContext.SaveChangesAsync(cancellationToken);
        }
    }
}
