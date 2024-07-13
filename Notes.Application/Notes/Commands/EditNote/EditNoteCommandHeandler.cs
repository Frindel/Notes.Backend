using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Notes.Application.Common.Exceptions;
using Notes.Application.Interfaces;
using Notes.Application.Notes.Commands.CreateNote;
using Notes.Application.Notes.Dto;
using Notes.Domain;
using System.Threading;

namespace Notes.Application.Notes.Commands.EditNote
{
    public class EditNoteCommandHeandler : IRequestHandler<EditNoteCommand, NoteDto>
    {
        INotesContext _notesContext;
        ICategoriesContext _categoriesContext;
        IUsersContext _usersContext;
        IMapper _mapper;

        public EditNoteCommandHeandler(INotesContext notesContext, ICategoriesContext categoriesContex,
            IUsersContext usersContext, IMapper mapper)
        {
            _notesContext = notesContext;
            _categoriesContext = categoriesContex;
            _usersContext = usersContext;
            _mapper = mapper;
        }

        public async Task<NoteDto> Handle(EditNoteCommand request, CancellationToken cancellationToken)
        {
            User user = await GetUserByIdAsync(request.UserId, cancellationToken);
            Note changedNote = await GetNoteByIdAsync(request.NoteId, user, cancellationToken);
            List<Category> categories = await GetCategoriesByIds(request.CategoriesIds, user, cancellationToken);
            
            Note updatedNote = await UpdateNoteAndSaveAsync(request, user, changedNote, categories, cancellationToken);
            return CreateTransferNote(updatedNote);
        }

        async Task<User> GetUserByIdAsync(int userId, CancellationToken cancellationToken)
        {
            User? selectedUser = await _usersContext.Users
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (selectedUser == null)
                throw new UserNotFoundException("User is not found");

            return selectedUser;
        }
        async Task<Note> GetNoteByIdAsync(int noteId, User targetUser, CancellationToken cancellationToken)
        {
            Note? selectedNote = await _notesContext.Notes
                .FirstOrDefaultAsync(n => n.PersonalId == noteId && n.User.Id == targetUser.Id, cancellationToken);
            if (selectedNote == null)
                throw new NoteNotFoundException($"Note with id = {noteId} not found");

            return selectedNote;
        }

        async Task<List<Category>> GetCategoriesByIds(List<int> categoriesIds, User targetUser, CancellationToken cancellationToken)
        {
            List<Category> selectedCategories = await _categoriesContext.Categories
                .Where(c => c.User.Id == targetUser.Id && categoriesIds.Contains(c.PersonalId))
                .ToListAsync(cancellationToken);
            if (selectedCategories.Count != categoriesIds.Count)
            {
                var selectedCategoryIds = new HashSet<int>(selectedCategories.Select(c => c.Id));
                List<int> notExistentCategoriesIds = categoriesIds.Where(c => !selectedCategoryIds.Contains(c)).ToList();

                throw new CategoryNotFoundException($"Category with ids: {string.Join(',', notExistentCategoriesIds)} not found");
            }

            return selectedCategories;
        }

        async Task<Note> UpdateNoteAndSaveAsync(EditNoteCommand request, User user, Note changedNote, 
            List<Category> categories, CancellationToken cancellationToken)
        {
            SetNoteValues(request, changedNote, categories);
            await SaveUpdatedNoteAsync(changedNote, cancellationToken);

            return changedNote;
        }

        Note SetNoteValues(EditNoteCommand request, Note changedNote, List<Category> categories)
        {
            changedNote.Name = request.Name;
            changedNote.Description = request.Description;
            changedNote.Time = request.Time;
            changedNote.IsCompleted = request.IsCompleted;
            changedNote.Categories = categories;

            return changedNote;
        }

        async Task SaveUpdatedNoteAsync(Note changedNote, CancellationToken cancellationToken)
        {
            _notesContext.Notes.Update(changedNote);
            await _notesContext.SaveChangesAsync(cancellationToken);
        }

        NoteDto CreateTransferNote(Note note)
        {
            return _mapper.Map<NoteDto>(note);
        }
    }
}
