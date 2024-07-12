using AutoMapper;
using Notes.Application.Common.Mapping;
using Notes.Application.Interfaces;
using Notes.Application.Notes.Commands.CreateNote;
using Notes.Domain;
using Notes.Persistence.Data;
using System.Text;

namespace Notes.ApplicationTests.Common
{
    internal abstract class TestBase : IDisposable
    {
        List<DataContext> _createdContexts;

        protected IMapper Mapper { get; }

        protected TestBase Helper { get; }

        public TestBase()
        {
            Mapper = GetConfiguredMapper();
            _createdContexts = new List<DataContext>();
            Helper = this;
        }

        IMapper GetConfiguredMapper()
        {
            var configurationProvider = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AssemblyMappingProfile(
                    typeof(INotesContext).Assembly));
            });
            return configurationProvider.CreateMapper();
        }

        protected DataContext CreateEmptyDataContex()
        {
            DataContext newContext = DataContextFactory.GetTestDataContextOnMemmory();
            _createdContexts.Add(newContext);

            return newContext;
        }

        #region Memmory cleaning

        public void Dispose()
        {
            foreach (DataContext context in _createdContexts)
                DataContextFactory.DestroyTestDataContexOnMemmory(context);
        }

        #endregion

        #region Helpers

        public string CreateRandomStr(int length)
        {
            string chars = "0123456789abcefghjkpqrstxyzABCEFGHJKPQRSTXYZ-_";
            StringBuilder builder = new StringBuilder();
            Random random = new Random();

            for (int index = 0; index < length; index++)
            {
                char rndChar = chars[random.Next(0, 45)];
                builder.Append(rndChar);
            }

            return builder.ToString();
        }

        protected List<User> AddUserWithNumbers(DataContext context, params int[] usersNumbers)
        {
            List<User> addedUsers = AddEntitiesWithNumbers(context, CreateUserOfNumber, usersNumbers);
            return addedUsers;
        }

        protected List<Category> AddCategoriesWithNumbers(DataContext context, User targetUser, params int[] categoriesNumbers)
        {
            List<Category> addedCategories = AddEntitiesWithNumbers(
                context,
                number => CreateCategoryOfNumber(number, targetUser),
                categoriesNumbers);

            return addedCategories;
        }

        List<T> AddEntitiesWithNumbers<T>(DataContext context, Func<int, T> createEntityFunc, params int[] numbers) where T : class
        {
            var newEntities = numbers
                .Select(createEntityFunc)
                .ToList();

            context.Set<T>().AddRange(newEntities);
            context.SaveChanges();

            return newEntities;
        }

        public User CreateUserOfNumber(int number)
        {
            return new User()
            {
                Id = number,
                Login = $"user {number}",
                Password = CreateRandomStr(10),
                RefreshToken = CreateRandomStr(256),
            };
        }

        public Category CreateCategoryOfNumber(int number, User user)
        {
            return new Category()
            {
                PersonalId = number,
                Name = $"category {number}",
                User = user
            };

        }

        #endregion
    }
}
