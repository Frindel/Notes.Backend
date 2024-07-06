using Microsoft.EntityFrameworkCore;
using Notes.Persistence.Data;

namespace Notes.ApplicationTests.Common
{
    internal static class DataContextFactory
    {
        public static DataContext GetTestDataContextOnMemmory()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            DataContext context = CreateContextAndTables(options);
            return context;
        }

        static DataContext CreateContextAndTables(DbContextOptions<DataContext> options)
        {
            DataContext context = new DataContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        public static void DestroyTestDataContexOnMemmory(DataContext context)
        {
            if (!context.Database.IsInMemory())
                throw new ArgumentException("database is not in memory");

            context.Database.EnsureDeleted();
            context.Dispose();
        }
    }
}
