using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Notes.Application.Common.Exceptions;
using Notes.Application.Interfaces;
using Notes.Application.Notes.Dto;
using Notes.Domain;

namespace Notes.Application.Notes.Queries.GetAllNotes
{
    public class GetAllNotesQueryHeandler : IRequestHandler<GetAllNotesQuery, NotesDto>
    {
        INotesContext _notesContext;
        IUsersContext _usersContext;
        IMapper _mapper;

        public GetAllNotesQueryHeandler(INotesContext notesContext, IUsersContext usersContext, IMapper mapper)
        {
            _notesContext = notesContext;
            _usersContext = usersContext;
            _mapper = mapper;
        }

        public async Task<NotesDto> Handle(GetAllNotesQuery request, CancellationToken cancellationToken)
        {
            User user = await GetUserById(request.UserId, cancellationToken);
            List<Note> userNotes = await GetNotesFor(user);

            return CreateTransferNotes(userNotes);
        }
         
        async Task<User> GetUserById(int userId, CancellationToken cancellationToken)
        {
            User? selectedUser = await _usersContext.Users
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (selectedUser == null)
                throw new UserNotFoundException("User is not found");

            return selectedUser;
        }

        async Task<List<Note>> GetNotesFor(User user)
        {
            List<Note> notes = await _notesContext.Notes
                .Where(n => n.User.Id == user.Id)
                .ToListAsync();
            return notes;
        }

        NotesDto CreateTransferNotes(List<Note> notes)
        {
            List<NoteDto> mappedNotes = notes
                .AsQueryable()
                .ProjectTo<NoteDto>(_mapper.ConfigurationProvider)
                .ToList();

            return new NotesDto
            {
                Notes = mappedNotes
            };
        }
    }
}
