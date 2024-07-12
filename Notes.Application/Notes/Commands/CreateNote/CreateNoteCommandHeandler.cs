using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Notes.Application.Common.Exceptions;
using Notes.Application.Interfaces;
using Notes.Application.Notes.Dto;
using Notes.Domain;

namespace Notes.Application.Notes.Commands.CreateNote
{
    public class CreateNoteCommandHeandler : IRequestHandler<CreateNoteCommand, NoteDto>
    {
        INotesContext _notesContext;
        ICategoriesContext _categoriesContext;
        IUsersContext _usersContext;
        IMapper _mapper;

        public CreateNoteCommandHeandler(INotesContext notesContext, ICategoriesContext categoriesContext, IUsersContext usersContext, IMapper mapper)
        {
            _notesContext = notesContext;
            _categoriesContext = categoriesContext;
            _usersContext = usersContext;
            _mapper = mapper;
        }

        public async Task<NoteDto> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
        {
            User targetUser = await GetUserById(request.UserId, cancellationToken);
            List<Category> categories = await GetCategoriesByIds(request.CategoriesIds, targetUser, cancellationToken);

            var newNote = await CreateAndSaveNote(request, targetUser, categories, cancellationToken);
            return CreateTransferNote(newNote);
        }

        async Task<User> GetUserById(int userId, CancellationToken cancellationToken)
        {
            User? selectedUser = await _usersContext.Users
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (selectedUser == null)
                throw new UserNotFoundException("User is not found");

            return selectedUser;
        }

        async Task<List<Category>> GetCategoriesByIds(List<int> categoriesIds, User targetUser, CancellationToken cancellationToken)
        {
            List<Category> selectedCategories = await _categoriesContext.Categories
                .Where(c => c.User.Id == targetUser.Id && categoriesIds.Contains(c.PersonalId))
                .ToListAsync(cancellationToken);
            if (selectedCategories.Count != categoriesIds.Count)
            {
                var selectedCategoryIds = new HashSet<int>(selectedCategories.Select(c => c.Id));
                List<int> notExistentCategoriesIds = categoriesIds.Where(c=> !selectedCategoryIds.Contains(c)).ToList();

                throw new CategoryNotFoundException($"Category with ids: {string.Join(',', notExistentCategoriesIds)} not found");
            }

            return selectedCategories;
        }

        async Task<Note> CreateAndSaveNote(CreateNoteCommand request, User user, List<Category> categories, CancellationToken cancellationToken)
        {
            int oldIndex = GetOldIndexNote(user.Id);
            Note newNote = new Note()
            {
                PersonalId = oldIndex + 1,
                Name = request.Name,
                Description = request.Description,
                Time = request.Time,
                IsCompleted = false,
                User = user,
                Categories = categories
            };
            _notesContext.Notes.Add(newNote);
            await _notesContext.SaveChangesAsync(cancellationToken);

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

        NoteDto CreateTransferNote(Note note)
        {
            return _mapper.Map<NoteDto>(note);
        }
    }
}
