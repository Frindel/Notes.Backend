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

		public GetAllCategoriesQueryHandler(UsersHelper usersHelper, ICategoriesContext categoriesContex, IMapper mapper)
		{
			_usersHelper = usersHelper;
			_categoriesContext = categoriesContex;
			_mapper = mapper;
		}

		public async Task<CategoriesDto> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
		{
			User selectedUser = await _usersHelper.GetUserByIdAsync(request.UserId);
			List<Category> categories = await SelectAllCategoriesForUserAsync(selectedUser, cancellationToken);
			return MapToDto(categories);
		}

		async Task<List<Category>> SelectAllCategoriesForUserAsync(User user, CancellationToken cancellationToken)
		{
			return await _categoriesContext.Categories
				.Where(c => c.User.Id == user.Id)
				.ToListAsync(cancellationToken);
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
