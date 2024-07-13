using MediatR;
using Notes.Application.Notes.Dto;

namespace Notes.Application.Notes.Commands.DeleteNote
{
    public sealed class DeleteNoteCommand : IRequest
    {
        public int UserId { get; set; }

        public int NoteId { get; set; }
    }
}
