using Microsoft.EntityFrameworkCore;
using Notes.Application.Common.Exceptions;
using Notes.Application.Interfaces;
using Notes.Domain;

namespace Notes.Application.Common.Helpers
{
    public class UsersHelper : HelperBase<User>
    {
        readonly IUsersContext _usersContext;
        readonly IJwtTokensService _jwtTokensService;
        readonly DbSet<User> _users;

        public UsersHelper(IUsersContext usersContext, IJwtTokensService jwtTokensService)
        {
            _usersContext = usersContext;
            _users = _usersContext.Users;
            _jwtTokensService = jwtTokensService;
        }

        public Task<User> GetUserByIdAsync(int userId) =>
            GetEntityByAsync(_users.Where(u => u.Id == userId),
                typeof(UserNotFoundException),
                $"user with id {userId} does not found");
        public Task<User> GetUserByLoginAsync(string login) =>
            GetEntityByAsync(_users.Where(u => u.Login == login),
                typeof(UserNotFoundException),
                "invalid user login or password");

        public async Task<User> SaveUserAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return await SaveEntityAsync(user, _users, () => _usersContext.SaveChangesAsync(cancellationToken));
        }

        public (string accessToken, string refreshToken) CreateTokens(int userId) =>
            (_jwtTokensService.GenerateAccessToken(userId), _jwtTokensService.GenerateRefrechToken(userId));
    }
}
