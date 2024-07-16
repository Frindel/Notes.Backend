﻿using AutoMapper;
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
			User selectedUser = await _usersHelper.GetUserByIdAsync(request.UserId);
			Category savedCategory = await CreateAndSaveCategoryAsync(request.CategoryName, selectedUser, cancellationToken);
			return MapToDto(savedCategory);
		}

		async Task<Category> CreateAndSaveCategoryAsync(string name, User user, CancellationToken cancellationToken)
		{
			int oldIndex = GetLastIndexCategoryForUser(user.Id);
			Category newCategory = new Category() { PersonalId = oldIndex + 1, Name = name, User = user };
			return await _cateriesHelper.SaveCategoryAsync(newCategory, cancellationToken);
		}

		int GetLastIndexCategoryForUser(int userId)
		{
			Category? oldICategory = _categoriesContext.Categories
				.Where(c => c.User.Id == userId)
				.OrderByDescending(c => c.PersonalId)
				.FirstOrDefault();
			return oldICategory == null ? 0 : oldICategory.Id;
		}

		CategoryDto MapToDto(Category category)
		{
			CategoryDto transferCategory = _mapper.Map<CategoryDto>(category);
			return transferCategory;
		}
	}
}
