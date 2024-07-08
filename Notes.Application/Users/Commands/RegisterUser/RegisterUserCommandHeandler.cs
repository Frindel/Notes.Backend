using AutoMapper;
using MediatR;
using Notes.Application.Common.Exceptions;
using Notes.Application.Interfaces;
using Notes.Domain;

namespace Notes.Application.Users.Commands.RegisterUser
{
    public class RegisterUserCommandHeandler : IRequestHandler<RegisterUserCommand, UserDto>
    {
        IUsersContext _users;
        IJwtTokensService _jwtTokens;
        IMapper _mapper;

        public RegisterUserCommandHeandler(IUsersContext users, IJwtTokensService jwtTokens, IMapper mapper)
        {
            _users = users;
            _jwtTokens = jwtTokens;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            bool userIsAlreadyExists = UserWithLoginIsExists(request.Login);
            if (userIsAlreadyExists)
                throw new UserIsAlreadyRegisteredException("User with this login has already been registered");

            UserDto newUser = await CreateNewUserAndSave(request, cancellationToken);
            return newUser;
        }

        bool UserWithLoginIsExists(string login)
        {
            User? existingUser = _users.Users.FirstOrDefault(u => u.Login == login);
            return existingUser != null;
        }

        async Task<UserDto> CreateNewUserAndSave(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            User newUser = new User()
            {
                Login = request.Login,
                Password = request.Password,
            };

            User savedUser = await SaveUser(newUser, cancellationToken);
            var tokens = CreateTokens(savedUser.Id);
            await SaveRefreshTokenForUser(savedUser, tokens.refreshToken, cancellationToken);

            return MergeUserData(newUser, tokens);
        }

        async Task<User> SaveUser(User user, CancellationToken token)
        {
            _users.Users.Add(user);
            await _users.SaveChangesAsync(token);
            return user;
        }

        (string accessToken, string refreshToken) CreateTokens(int userId)
        {
            string accessToken = _jwtTokens.GenerateAccessToken(userId);
            string refreshToken = _jwtTokens.GenerateRefrechToken(userId);
            return (accessToken, refreshToken);
        }

        async Task SaveRefreshTokenForUser(User user, string refreshToken, CancellationToken cancellationToken)
        {
            user.RefreshToken = refreshToken;

            _users.Users.Update(user);
            await _users.SaveChangesAsync(cancellationToken);
        }

        UserDto MergeUserData(User newUser, (string accessToken, string refreshToken) tokens)
        {
            UserDto addedUser = _mapper.Map<UserDto>(newUser);
            addedUser.AccessToken = tokens.accessToken;
            return addedUser;
        }
    }
}
