using Microsoft.EntityFrameworkCore;

namespace Notes.Application.Common.Helpers
{
    public abstract class HelperBase<T> where T : class
    {
        protected async Task<T> GetEntityByAsync(IQueryable<T> filter, Type exceptionType, string exceptionMessage)
        {
            if (exceptionType.BaseType != typeof(ApplicationException))
                throw new ArgumentException($"{nameof(exceptionType)} is not a exception");

            T? selectedEntity = await filter.FirstOrDefaultAsync();
            if (selectedEntity == null)
                throw (ApplicationException)Activator.CreateInstance(exceptionType, exceptionMessage)!;

            return selectedEntity;
        }

        protected async Task<T> SaveEntityAsync(T entity, DbSet<T> storage, Func<Task<int>> saveChanges)
        {
            if (storage.Contains(entity))
                storage.Update(entity);
            else
                storage.Add(entity);

            await saveChanges();
            return entity;
        }
    }
}
