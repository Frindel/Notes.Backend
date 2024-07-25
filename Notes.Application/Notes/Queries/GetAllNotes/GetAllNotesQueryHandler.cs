using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Notes.Application.Common.Helpers;
using Notes.Application.Interfaces;
using Notes.Application.Notes.Dto;
using Notes.Domain;

namespace Notes.Application.Notes.Queries.GetAllNotes
{
    public class GetAllNotesQueryHandler : IRequestHandler<GetAllNotesQuery, NotesDto>
    {
        readonly UsersHelper _usersHelper;
        readonly INotesContext _notesContext;
        readonly IMapper _mapper;

        public GetAllNotesQueryHandler(UsersHelper usersHelper, INotesContext notesContext, IMapper mapper)
        {
            _usersHelper = usersHelper;
            _notesContext = notesContext;
            _mapper = mapper;
        }

        public async Task<NotesDto> Handle(GetAllNotesQuery request, CancellationToken cancellationToken)
        {
            User user = await _usersHelper.GetUserByIdAsync(request.UserId);
            List<Note> userNotes = await GetNotesFor(user);

            return MapToDto(userNotes);
        }

        async Task<List<Note>> GetNotesFor(User user)
        {
            List<Note> notes = await _notesContext.Notes.Include(n => n.Categories)
                .Where(n => n.User.Id == user.Id)
                .ToListAsync();
            return notes;
        }

        NotesDto MapToDto(List<Note> notes)
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