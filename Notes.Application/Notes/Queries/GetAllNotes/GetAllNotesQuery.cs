using MediatR;
using Notes.Application.Notes.Dto;

namespace Notes.Application.Notes.Queries.GetAllNotes
{
    public record class GetAllNotesQuery : IRequest<NotesDto>
    {
        public int UserId { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }
}