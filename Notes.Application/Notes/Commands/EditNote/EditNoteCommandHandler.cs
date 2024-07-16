using AutoMapper;
using MediatR;
using Notes.Application.Common.Helpers;
using Notes.Application.Notes.Dto;
using Notes.Domain;

namespace Notes.Application.Notes.Commands.EditNote
{
    public class EditNoteCommandHandler : IRequestHandler<EditNoteCommand, NoteDto>
    {
        readonly UsersHelper _usersHelper;
        readonly CategoriesHelper _categoriesHelper;
        readonly NotesHelper _notesHelper;
        readonly IMapper _mapper;

        public EditNoteCommandHandler(UsersHelper usersHelper, CategoriesHelper categoriesHelper, NotesHelper notesHelper, IMapper mapper)
        {
            _usersHelper = usersHelper;
            _categoriesHelper = categoriesHelper;
            _notesHelper = notesHelper;
            _mapper = mapper;
        }

        public async Task<NoteDto> Handle(EditNoteCommand request, CancellationToken cancellationToken)
        {
            User user = await _usersHelper.GetUserByIdAsync(request.UserId);
            Note changingNote = await _notesHelper.GetNoteByIdAsync(request.NoteId, user.Id);
            List<Category> categories = await _categoriesHelper.GetCategoriesByIdsAsync(request.CategoriesIds, user.Id);

            Note updatedNote = await UpdateNoteAndSaveAsync(request, changingNote, categories, cancellationToken);
            return MapToDto(updatedNote);
        }

        async Task<Note> UpdateNoteAndSaveAsync(EditNoteCommand request, Note changingNote,
            List<Category> categories, CancellationToken cancellationToken)
        {
            SetValuesToNote(request, changingNote, categories);
            await _notesHelper.SaveNoteAsync(changingNote, cancellationToken);
            return changingNote;
        }

        Note SetValuesToNote(EditNoteCommand request, Note changedNote, List<Category> categories)
        {
            changedNote.Name = request.Name;
            changedNote.Description = request.Description;
            changedNote.Time = request.Time;
            changedNote.IsCompleted = request.IsCompleted;
            changedNote.Categories = categories;

            return changedNote;
        }

        NoteDto MapToDto(Note note)
        {
            return _mapper.Map<NoteDto>(note);
        }
    }
}
