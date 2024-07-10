using AutoMapper;
using MediatR;
using Notes.Application.Categories.Dto;
using Notes.Application.Common.Exceptions;
using Notes.Application.Interfaces;
using Notes.Domain;

namespace Notes.Application.Categories.Command.CreateCategory
{
    public class CreateCategoryCommandHeandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
    {
        IUsersContext _usersContext;
        ICategoriesContext _categoriesContext;
        IMapper _mapper;

        public CreateCategoryCommandHeandler(IUsersContext usersContext, ICategoriesContext categoriesContext, IMapper mapper)
        {
            _usersContext = usersContext;
            _categoriesContext = categoriesContext;
            _mapper = mapper;
        }

        public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            User selectedUser = GetUserById(request.UserId);

            Category savedCategory = await CreateAndSaveCategory(request.CategoryName, selectedUser, cancellationToken);
            return CreateTransferCategory(savedCategory);
        }

        User GetUserById(int userId)
        {
            User? user = _usersContext.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
                throw new UserNotFoundException($"user with id = {userId} was not found");

            return user;
        }

        async Task<Category> CreateAndSaveCategory(string name, User user, CancellationToken cancellationToken)
        {
            int oldIndex = GetOldIndexCategory(user.Id);
            Category newCategory = CreateCategory(oldIndex + 1, name, user);

            Category savedCategory = await SaveCategory(newCategory, cancellationToken);
            return savedCategory;
        }

        int GetOldIndexCategory(int userId)
        {
            Category? oldICategory = _categoriesContext.Categories
                .Where(c => c.User.Id == userId)
                .OrderByDescending(c => c.PersonalId)
                .FirstOrDefault();

            return oldICategory == null ? 0 : oldICategory.Id;
        }

        Category CreateCategory(int personId, string name, User user)
        {
            return new Category()
            {
                PersonalId = personId,
                Name = name,
                User = user
            };
        }

        async Task<Category> SaveCategory(Category category, CancellationToken cancellationToken)
        {
            _categoriesContext.Categories.Add(category);
            await _categoriesContext.SaveChangesAsync(cancellationToken);
            return category;
        }

        CategoryDto CreateTransferCategory(Category category)
        {
            CategoryDto transferCategory = _mapper.Map<CategoryDto>(category);
            return transferCategory;
        }
    }
}
