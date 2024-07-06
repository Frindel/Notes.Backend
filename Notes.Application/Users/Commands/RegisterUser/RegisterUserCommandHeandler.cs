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
        ITokensGenerator _tokensGenerator;
        IMapper _mapper;

        public RegisterUserCommandHeandler(IUsersContext users, ITokensGenerator tokensGenerator, IMapper mapper)
        {
            _users = users;
            _tokensGenerator = tokensGenerator;
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

        async Task<UserDto> CreateNewUserAndSave(RegisterUserCommand request, CancellationToken token)
        {
            string refreshToken = _tokensGenerator.GenerateRefrechToken();
            User newUser = new User()
            {
                Login = request.Login,
                Password = request.Password,
                RefreshToken = refreshToken
            };

            await SaveUser(newUser, token);
            string accessToken = GetAccessToken(newUser);

            return MergeUserData(newUser, accessToken);
        }

        async Task<User> SaveUser(User user, CancellationToken token)
        {
            _users.Users.Add(user);
            await _users.SaveChangesAsync(token);
            return user;
        }

        string GetAccessToken(User user)
        {
            return _tokensGenerator.GenerateAccessToken(user.Id);
        }

        UserDto MergeUserData(User newUser, string accessToken)
        {
            UserDto addedUser = _mapper.Map<UserDto>(newUser);
            addedUser.AccessToken = accessToken;
            return addedUser;
        }
    }
}
