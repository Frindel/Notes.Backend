using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Notes.Application.Common.Helpers;
using Notes.Application.Interfaces;
using Notes.Application.Notes.Dto;
using Notes.Domain;

namespace Notes.Application.Notes.Queries.GetAllNotes
{
    public class GetAllNotesQueryHandler : IRequestHandler<GetAllNotesQuery, NotesDto>
    {
        readonly UsersHelper _usersHelper;
        readonly INotesContext _notesContext;
        private CategoriesHelper _categoriesHelper;
        readonly IMapper _mapper;

        private CancellationToken _cancellationToken;

        public GetAllNotesQueryHandler(UsersHelper usersHelper, CategoriesHelper categoriesHelper,
            INotesContext notesContext, IMapper mapper)
        {
            _usersHelper = usersHelper;
            _notesContext = notesContext;
            _categoriesHelper = categoriesHelper;
            _mapper = mapper;
        }

        public async Task<NotesDto> Handle(GetAllNotesQuery request, CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            User user = await _usersHelper.GetUserByIdAsync(request.UserId);

            // проверка существования категорий с переданными id. При их отсутствии возникает исключение NotFoundException
            await CheckExistenceOfCategories(request.CategoriesIds, user.Id);
            List<Note> userNotes =
                await GetNotesFor(user.Id, request.PageNumber, request.PageSize, request.CategoriesIds);

            return MapToDto(userNotes);
        }

        async Task CheckExistenceOfCategories(List<int> categoriesIds, int userId)
        {
            await _categoriesHelper.GetCategoriesByIdsAsync(categoriesIds, userId);
        }

        async Task<List<Note>> GetNotesFor(int userId, int pageNumber, int pageSize, List<int> categoriesIds)
        {
            int startIndex = (pageNumber - 1) * pageSize;
            var query = BuildNotesQueryWithCategories(categoriesIds, userId);

            List<Note> notes = await query
                .Skip(startIndex)
                .Take(pageSize)
                .ToListAsync(_cancellationToken);

            return notes;
        }

        IQueryable<Note> BuildNotesQueryWithCategories(List<int> categoriesIds, int userId)
        {
            IQueryable<Note> query = _notesContext.Notes.Include(n => n.Categories)
                .Where(n => n.User.Id == userId);

            if (categoriesIds.Count == 0)
                return query;
            return query.Where(n => n.Categories.Any(c => categoriesIds.Contains(c.PersonalId)));
        }

        NotesDto MapToDto(List<Note> notes)
        {
            List<NoteDto> mappedNotes = notes
                .AsQueryable()
                .ProjectTo<NoteDto>(_mapper.ConfigurationProvider)
                .ToList();

            return new NotesDto
            {
                Notes = mappedNotes
            };
        }
    }
}