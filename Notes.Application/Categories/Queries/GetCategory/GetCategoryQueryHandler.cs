using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Notes.Application.Categories.Dto;
using Notes.Application.Common.Exceptions;
using Notes.Application.Common.Helpers;
using Notes.Application.Interfaces;
using Notes.Domain;

namespace Notes.Application.Categories.Queries.GetCategory
{
    public class GetCategoryQueryHandler : IRequestHandler<GetCategoryQuery, CategoryDto>
    {
        readonly UsersHelper _usersHelper;
        readonly ICategoriesContext _categoriesContext;
        readonly IUsersContext _usersContext;
        readonly IMapper _mapper;

        public GetCategoryQueryHandler(UsersHelper usersHelper, ICategoriesContext categoriesContext,
            IUsersContext usersContext, IMapper mapper)
        {
            _usersHelper = usersHelper;
            _categoriesContext = categoriesContext;
            _usersContext = usersContext;
            _mapper = mapper;
        }

        public async Task<CategoryDto> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
        {
            var users = _usersContext.Users.ToList();

            User selectedUser = await _usersHelper.GetUserByIdAsync(request.UserId);
            Category selectedCategory = await GetCategoryById(request.CategoryId, selectedUser);
            return MapToDto(selectedCategory);
        }

        async Task<Category> GetCategoryById(int categoryId, User forUser)
        {
            Category? selectedCategory = await _categoriesContext.Categories
                .FirstOrDefaultAsync(c => c.User.Id == forUser.Id && c.PersonalId == categoryId);
            if (selectedCategory == null)
                throw new NotFoundException($"category with id = {categoryId} not found");

            return selectedCategory;
        }

        CategoryDto MapToDto(Category category)
        {
            CategoryDto transferCategory = _mapper.Map<CategoryDto>(category);
            return transferCategory;
        }
    }
}