using System.Drawing;
using AutoMapper;
using FluentValidation;
using MediatR;
using Notes.Application.Categories.Dto;
using Notes.Application.Common.Helpers;
using Notes.Application.Interfaces;
using Notes.Domain;

namespace Notes.Application.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
    {
        readonly UsersHelper _usersHelper;
        readonly CategoriesHelper _cateriesHelper;
        readonly ICategoriesContext _categoriesContext;
        readonly IMapper _mapper;

        private CancellationToken _cancellationToken = CancellationToken.None;

        public CreateCategoryCommandHandler(UsersHelper usersHelper, CategoriesHelper categoriesHelper,
            ICategoriesContext categoriesContext, IMapper mapper)
        {
            _usersHelper = usersHelper;
            _cateriesHelper = categoriesHelper;
            _categoriesContext = categoriesContext;
            _mapper = mapper;
        }

        public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            string lowerCategoryName = request.CategoryName.ToLower();
            if (CategoryWithNameIsExists(lowerCategoryName))
                throw new ValidationException("category with this name is already exists");

            User selectedUser = await _usersHelper.GetUserByIdAsync(request.UserId);
            Category savedCategory =
                await CreateAndSaveCategoryAsync(request, selectedUser);
            return MapToDto(savedCategory);
        }

        bool CategoryWithNameIsExists(string categoryName) =>
            _categoriesContext.Categories.FirstOrDefault(c => c.Name == categoryName) != null;


        async Task<Category> CreateAndSaveCategoryAsync(CreateCategoryCommand request, User user)
        {
            int oldIndex = GetLastIndexCategoryForUser(user.Id);
            Category newCategory = new Category()
            {
                PersonalId = oldIndex + 1,
                Name = request.CategoryName,
                Color = ColorTranslator.FromHtml(request.Color),
                User = user
            };
            return await _cateriesHelper.SaveCategoryAsync(newCategory, _cancellationToken);
        }

        int GetLastIndexCategoryForUser(int userId)
        {
            Category? oldICategory = _categoriesContext.Categories
                .Where(c => c.User.Id == userId)
                .OrderByDescending(c => c.PersonalId)
                .FirstOrDefault();
            return oldICategory == null! ? 0 : oldICategory.Id;
        }

        CategoryDto MapToDto(Category category)
        {
            CategoryDto transferCategory = _mapper.Map<CategoryDto>(category);
            return transferCategory;
        }
    }
}