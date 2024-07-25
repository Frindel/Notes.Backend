using AutoMapper;
using MediatR;
using Notes.Application.Common.Helpers;
using Notes.Application.Notes.Dto;
using Notes.Domain;

namespace Notes.Application.Notes.Queries.GetNote
{
    public class GetNoteQueryHandler : IRequestHandler<GetNoteQuery, NoteDto>
    {
        readonly UsersHelper _usersHelper;
        readonly NotesHelper _notesHelper;
        readonly IMapper _mapper;

        public GetNoteQueryHandler(UsersHelper usersHelper, NotesHelper notesHelper, IMapper mapper)
        {
            _usersHelper = usersHelper;
            _notesHelper = notesHelper;
            _mapper = mapper;
        }

        public async Task<NoteDto> Handle(GetNoteQuery request, CancellationToken cancellationToken)
        {
            User user = await _usersHelper.GetUserByIdAsync(request.UserId);
            Note note = await _notesHelper.GetNoteByIdAsync(request.NoteId, user.Id);
            return MapToDto(note);
        }

        NoteDto MapToDto(Note note)
        {
            return _mapper.Map<NoteDto>(note);
        }
    }
}