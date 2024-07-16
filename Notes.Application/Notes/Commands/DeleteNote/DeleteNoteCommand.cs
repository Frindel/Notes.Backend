using MediatR;

namespace Notes.Application.Notes.Commands.DeleteNote
{
    public record class DeleteNoteCommand : IRequest
    {
        public int UserId { get; set; }

        public int NoteId { get; set; }
    }
}
