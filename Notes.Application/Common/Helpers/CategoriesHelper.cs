using Microsoft.EntityFrameworkCore;
using Notes.Application.Common.Exceptions;
using Notes.Application.Interfaces;
using Notes.Domain;

namespace Notes.Application.Common.Helpers
{
    public class CategoriesHelper : HelperBase<Category>
    {
        readonly ICategoriesContext _categoriesContext;
        readonly DbSet<Category> _categories;

        public CategoriesHelper(ICategoriesContext categoriesContext)
        {
            _categoriesContext = categoriesContext;
            _categories = _categoriesContext.Categories;
        }

        public Task<Category> GetCategoryByIdAsync(int categoryId, int forUserId) =>
             GetEntityByAsync(
                _categories.Where(c => c.PersonalId == categoryId && c.User.Id == forUserId),
                typeof(CategoryNotFoundException),
                $"Category with id {categoryId} and user id {forUserId} does not found");


        public async Task<Category> SaveCategoryAsync(Category category, CancellationToken cancellationToken)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));
            if (category.User == null)
                throw new ArgumentException(nameof(category.User));

            return await SaveEntityAsync(category, _categories, () => _categoriesContext.SaveChangesAsync(cancellationToken));
        }

        public async Task<List<Category>> GetCategoriesByIdsAsync(List<int> categoriesIds, int targetUserId)
        {
            List<Category> selectedCategories = await _categories
                .Where(c => c.User.Id == targetUserId && categoriesIds.Contains(c.PersonalId))
                .ToListAsync();
            if (selectedCategories.Count != categoriesIds.Count)
            {
                var selectedCategoryIds = new HashSet<int>(selectedCategories.Select(c => c.Id));
                List<int> notExistentCategoriesIds = categoriesIds.Where(c => !selectedCategoryIds.Contains(c)).ToList();

                throw new CategoryNotFoundException($"Category with ids: {string.Join(',', notExistentCategoriesIds)} not found");
            }

            return selectedCategories;
        }
    }
}
