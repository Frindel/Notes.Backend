using MediatR;
using Notes.Application.Common.Helpers;
using Notes.Application.Interfaces;
using Notes.Domain;

namespace Notes.Application.Notes.Commands.DeleteNote
{
    public class DeleteNoteCommandHandler : IRequestHandler<DeleteNoteCommand, int>
    {
        readonly UsersHelper _usersHelper;
        readonly NotesHelper _notesHelper;
        readonly INotesContext _notesContext;
        private readonly ICategoriesContext _categoriesContext;

        private CancellationToken _cancellationToken = CancellationToken.None;

        public DeleteNoteCommandHandler(UsersHelper usersHelper, NotesHelper notesHelper,
            INotesContext notesContext, ICategoriesContext categoriesContext)
        {
            _usersHelper = usersHelper;
            _notesHelper = notesHelper;
            _notesContext = notesContext;
            _categoriesContext = categoriesContext;
        }

        public async Task<int> Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;

            User user = await _usersHelper.GetUserByIdAsync(request.UserId);
            Note deletedNote = await _notesHelper.GetNoteByIdAsync(request.NoteId, user.Id);
            await DeleteNoteAsync(deletedNote);

            // удаление категорий при их отсутствии в других заметках пользователя
            await TryDeleteCategoriesAsync(deletedNote.Categories);

            return deletedNote.PersonalId;
        }

        async Task DeleteNoteAsync(Note deletedNote)
        {
            _notesContext.Notes.Remove(deletedNote);
            await _notesContext.SaveChangesAsync(_cancellationToken);
        }

        async Task TryDeleteCategoriesAsync(List<Category> categories)
        {
            List<Category> unusedCategories = categories.Where(c => c.Notes.Count == 0).ToList();
            _categoriesContext.Categories.RemoveRange(unusedCategories);
            await _categoriesContext.SaveChangesAsync(_cancellationToken);
        }
    }
}