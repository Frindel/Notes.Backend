using MediatR;
using Notes.Application.Notes.Dto;

namespace Notes.Application.Notes.Queries.GetAllNotes
{
    public sealed class GetAllNotesQuery : IRequest<NotesDto>
    {
        public int UserId { get; set; }
    }
}
