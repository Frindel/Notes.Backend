using AutoMapper;
using MediatR;
using Notes.Application.Interfaces;
using Notes.Application.Notes.Dto;
using Notes.Domain;
using Notes.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Notes.Application.Notes.Queries.GetNote
{
    public class GetNoteQueryHeandler : IRequestHandler<GetNoteQuery, NoteDto>
    {
        IUsersContext _usersContext;
        INotesContext _notesContext;
        IMapper _mapper;

        public GetNoteQueryHeandler(IUsersContext usersContext, INotesContext notesContext, IMapper mapper)
        {
            _usersContext = usersContext;
            _notesContext = notesContext;
            _mapper = mapper;
        }

        public async Task<NoteDto> Handle(GetNoteQuery request, CancellationToken cancellationToken)
        {
            // получение пользователя
            User user = await GetUserById(request.UserId, cancellationToken);
            // получение заметки
            Note note = await GetNote(request.NoteId, user, cancellationToken);
            // маппинг
            return MapNoteToTransferObj(note);
        }

        async Task<User> GetUserById(int userId, CancellationToken cancellationToken)
        {
            User? selectedUser = await _usersContext.Users
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (selectedUser == null)
                throw new UserNotFoundException($"user with id = {userId} not found");

            return selectedUser;
        }

        async Task<Note> GetNote(int noteId, User user, CancellationToken cancellationToken)
        {
            Note? selectedNote = await _notesContext.Notes
                .FirstOrDefaultAsync(n=>n.User.Id == user.Id && n.PersonalId == noteId, cancellationToken);
            if (selectedNote== null)
                throw new NoteNotFoundException($"note with id = {noteId}");

            return selectedNote;
        }

        NoteDto MapNoteToTransferObj(Note note)
        {
            return _mapper.Map<NoteDto>(note);
        }
    }
}
