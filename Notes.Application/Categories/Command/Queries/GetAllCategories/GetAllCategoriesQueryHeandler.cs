using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Notes.Application.Categories.Dto;
using Notes.Application.Common.Exceptions;
using Notes.Application.Interfaces;
using Notes.Domain;

namespace Notes.Application.Categories.Command.Queries.GetAllCategories
{
    public class GetAllCategoriesQueryHeandler : IRequestHandler<GetAllCategoriesQuery, CategoriesDto>
    {
        IUsersContext _usersContext;
        ICategoriesContext _categoriesContext;
        IMapper _mapper;

        public GetAllCategoriesQueryHeandler(IUsersContext usersContext, ICategoriesContext categoriesContex, IMapper mapper)
        {
            _usersContext = usersContext;
            _categoriesContext = categoriesContex;
            _mapper = mapper;
        }

        public async Task<CategoriesDto> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            User selectedUser = await GetUserByIdAsync(request.UserId, cancellationToken);
            List<Category> categories = await SelectAllCategoriesForUserAsync(selectedUser, cancellationToken);

            return MapToTransferObj(categories);
        }

        async Task<User> GetUserByIdAsync(int userId, CancellationToken cancellationToken)
        {
            User? selectedUser = await _usersContext.Users
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (selectedUser == null)
                throw new UserNotFoundException($"user not found");

            return selectedUser;
        }

        async Task<List<Category>> SelectAllCategoriesForUserAsync(User user, CancellationToken cancellationToken)
        {
            return await _categoriesContext.Categories
                .Where(c => c.User.Id == user.Id)
                .ToListAsync(cancellationToken);
        }

        CategoriesDto MapToTransferObj(List<Category> categories)
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
