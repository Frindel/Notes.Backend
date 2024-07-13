using MediatR;
using Notes.Application.Notes.Dto;

namespace Notes.Application.Notes.Queries.GetNote
{
    public sealed class GetNoteQuery : IRequest<NoteDto>
    {
        public int UserId { get; set; }

        public int NoteId { get; set; }
    }
}
