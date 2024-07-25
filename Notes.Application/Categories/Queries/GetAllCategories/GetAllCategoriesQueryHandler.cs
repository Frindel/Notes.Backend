using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Notes.Application.Categories.Dto;
using Notes.Application.Common.Helpers;
using Notes.Application.Interfaces;
using Notes.Domain;

namespace Notes.Application.Categories.Queries.GetAllCategories
{
    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, CategoriesDto>
    {
        readonly UsersHelper _usersHelper;
        readonly ICategoriesContext _categoriesContext;
        readonly IMapper _mapper;

        private CancellationToken _cancellationToken = CancellationToken.None;

        public GetAllCategoriesQueryHandler(UsersHelper usersHelper, ICategoriesContext categoriesContext,
            IMapper mapper)
        {
            _usersHelper = usersHelper;
            _categoriesContext = categoriesContext;
            _mapper = mapper;
        }

        public async Task<CategoriesDto> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            User selectedUser = await _usersHelper.GetUserByIdAsync(request.UserId);
            List<Category> categories = await SelectAllCategoriesForUserAsync(selectedUser);
            return MapToDto(categories);
        }

        async Task<List<Category>> SelectAllCategoriesForUserAsync(User user)
        {
            return await _categoriesContext.Categories
                .Where(c => c.User.Id == user.Id)
                .ToListAsync(_cancellationToken);
        }

        CategoriesDto MapToDto(List<Category> categories)
        {
            List<CategoryDto> mappedCategories = categories
                .AsQueryable()
                .ProjectTo<CategoryDto>(_mapper.ConfigurationProvider)
                .ToList();

            return new CategoriesDto
            {
                Categories = mappedCategories
            };
        }
    }
}