using Microsoft.AspNetCore.Http;
using Notes.Application.Interfaces;
using Notes.Domain;
using System.Security.Claims;

namespace Notes.Persistence
{
    internal class CurrentUserService : ICurrentUser
    {
        private IUsersContext _users;
        private IHttpContextAccessor _httpContext;

        public User CurrentUser => GetUser();

        public CurrentUserService(IUsersContext users, IHttpContextAccessor httpContext)
        {
            _users = users;
            _httpContext = httpContext;
        }

        private User GetUser()
        {
            // todo: получение id пользователя из запроса
            var userClaim = _httpContext.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userClaim == null)
                throw new ArgumentException(nameof(userClaim));

            int userId = int.Parse(userClaim);

            // получение данных пользователя из БД
            User? currentUser = _users.Users.FirstOrDefault(u => u.Id == userId);

            if (currentUser == null)
                throw new ArgumentException(nameof(currentUser));

            return currentUser;
        }
    }
}
