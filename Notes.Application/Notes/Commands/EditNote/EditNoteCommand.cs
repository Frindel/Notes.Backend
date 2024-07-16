using MediatR;
using Notes.Application.Notes.Dto;

namespace Notes.Application.Notes.Commands.EditNote
{
    public record class EditNoteCommand : IRequest<NoteDto>
    {
        public int UserId { get; set; }

        public int NoteId { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public DateTime Time { get; set; }
        
        public bool IsCompleted { get; set; }

        public List<int> CategoriesIds { get; set; } = null!;
    }
}
