using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Notes.Application.Categories.Dto;
using Notes.Application.Common.Exceptions;
using Notes.Application.Interfaces;
using Notes.Domain;

namespace Notes.Application.Categories.Command.Queries.GetCategory
{
    public class GetCategoryQueryHeandler : IRequestHandler<GetCategoryQuery, CategoryDto>
    {
        ICategoriesContext _categoriesContext;
        IUsersContext _usersContext;
        IMapper _mapper;

        public GetCategoryQueryHeandler(ICategoriesContext categoriesContext, IUsersContext usersContext, IMapper mapper)
        {
            _categoriesContext = categoriesContext;
            _usersContext = usersContext;
            _mapper = mapper;
        }
        public async Task<CategoryDto> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
        {
            User selectedUser = await GetUserById(request.UserId, cancellationToken);
            Category selectedCategory = await GetCategoryById(request.CategoryId, selectedUser, cancellationToken);

            return CreateTransferCategory(selectedCategory);
        }

        async Task<User> GetUserById(int userId, CancellationToken cancellationToken)
        {
            User? selectredUser = await _usersContext.Users
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (selectredUser == null)
                throw new UserNotFoundException($"user with id = {userId} not found");

            return selectredUser;
        }

        async Task<Category> GetCategoryById(int categoryId, User forUser, CancellationToken cancellationToken)
        {
            Category? selectedCategory = await _categoriesContext.Categories
                .FirstOrDefaultAsync(c => c.User.Id == forUser.Id && c.PersonalId == categoryId);
            if (selectedCategory == null)
                throw new CategoryNotFoundException($"category with id = {categoryId} not found");

            return selectedCategory;
        }

        CategoryDto CreateTransferCategory(Category category)
        {
            CategoryDto transferCategory = _mapper.Map<CategoryDto>(category);
            return transferCategory;
        }
    }
}
