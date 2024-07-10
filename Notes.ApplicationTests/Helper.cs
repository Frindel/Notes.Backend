using Notes.Domain;
using System.Text;

namespace Notes.ApplicationTests
{
    internal static class Helper
    {
        public static string CreateRandomStr(int length)
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

        public static User CreateUserOfNumber(int number)
        {
            return new User()
            {
                Id = number,
                Login = $"user {number}",
                Password = CreateRandomStr(10),
                RefreshToken = CreateRandomStr(256),
            };
        }

        public static Category CreateCategoryOfNumber(int number, User user)
        {
            return new Category()
            {
                Id = number,
                Name = $"category {number}",
                User = user
            };

        }
    }
}
