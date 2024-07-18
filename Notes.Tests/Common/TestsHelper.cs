using Microsoft.AspNetCore.Http;
using Notes.Domain;
using Notes.Persistence.Data;
using System.Security.Claims;
using System.Text;

namespace Notes.ApplicationTests.Common
{
    internal class TestsHelper
    {
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

        public List<User> AddUserWithNumbers(DataContext context, params int[] usersNumbers)
        {
            List<User> addedUsers = AddEntitiesWithNumbers(context, CreateUserOfNumber, usersNumbers);
            return addedUsers;
        }

        public List<Category> AddCategoriesWithNumbers(DataContext context, User targetUser, params int[] categoriesNumbers)
        {
            List<Category> addedCategories = AddEntitiesWithNumbers(
                context,
                number => CreateCategoryOfNumber(number, targetUser),
                categoriesNumbers);

            return addedCategories;
        }

        public List<Note> AddNotesWithNumbers(DataContext contex, User targetUser, List<Category> categories, params int[] notesNumbers)
        {
            if (categories == null)
                categories = new List<Category>();

            List<Note> addedNotes = AddEntitiesWithNumbers(
                contex,
                number => CreateNoteOfNumber(number, targetUser, categories),
                notesNumbers);
            return addedNotes;
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

        public Note CreateNoteOfNumber(int number, User user, List<Category> categories = null!)
        {
            if (categories == null)
                categories = new List<Category>();

            return new Note()
            {
                PersonalId = number,
                Name = $"note {number}",
                Description = $"note description {number}",
                IsCompleted = false,
                Time = DateTime.UtcNow,
                User = user,
                Categories = categories
            };
        }

        public HttpContext SetUserIdForIdentity(HttpContext context, int userId)
        {
            var userIdClaim = new Claim(ClaimTypes.NameIdentifier, userId.ToString());
            var claimsIdentity = new ClaimsIdentity(new[] { userIdClaim });
            context.User.AddIdentity(claimsIdentity);
            return context;
        }
    }
}
