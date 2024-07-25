using AutoMapper;
using MediatR;
using Notes.Application.Common.Helpers;
using Notes.Application.Interfaces;
using Notes.Application.Notes.Dto;
using Notes.Domain;

namespace Notes.Application.Notes.Commands.CreateNote
{
    public class CreateNoteCommandHandler : IRequestHandler<CreateNoteCommand, NoteDto>
    {
        readonly UsersHelper _usersHelper;
        readonly CategoriesHelper _categoriesHalper;
        readonly NotesHelper _notesHalper;

        readonly INotesContext _notesContext;
        readonly IMapper _mapper;

        private CancellationToken _cancellationToken = CancellationToken.None;

        public CreateNoteCommandHandler(
            UsersHelper usersHelper, CategoriesHelper categoriesHelper, NotesHelper notesHelper,
            INotesContext notesContext, IMapper mapper)
        {
            _usersHelper = usersHelper;
            _categoriesHalper = categoriesHelper;
            _notesHalper = notesHelper;

            _notesContext = notesContext;
            _mapper = mapper;
        }

        public async Task<NoteDto> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            User forUser = await _usersHelper.GetUserByIdAsync(request.UserId);
            List<Category> categories =
                await _categoriesHalper.GetCategoriesByIdsAsync(request.CategoriesIds, forUser.Id);

            var newNote = await CreateAndSaveNoteAsync(request, forUser, categories);
            return MapToDto(newNote);
        }

        async Task<Note> CreateAndSaveNoteAsync(CreateNoteCommand request, User forUser, List<Category> categories)
        {
            int oldIndex = GetOldIndexNote(forUser.Id);
            Note newNote = new Note()
            {
                PersonalId = oldIndex + 1,
                Name = request.Name,
                Description = request.Description,
                Time = request.Time,
                IsCompleted = false,
                User = forUser,
                Categories = categories
            };
            await _notesHalper.SaveNoteAsync(newNote, _cancellationToken);

            return newNote;
        }

        int GetOldIndexNote(int userId)
        {
            Note? oldICategory = _notesContext.Notes
                .Where(n => n.User.Id == userId)
                .OrderByDescending(n => n.PersonalId)
                .FirstOrDefault();

            return oldICategory == null ? 0 : oldICategory.Id;
        }

        NoteDto MapToDto(Note note)
        {
            return _mapper.Map<NoteDto>(note);
        }
    }
}