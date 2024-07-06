using Notes.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.ApplicationTests.Users
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
                Password = Helper.CreateRandomStr(10),
                RefreshToken = Helper.CreateRandomStr(256),
            };
        }
    }
}
