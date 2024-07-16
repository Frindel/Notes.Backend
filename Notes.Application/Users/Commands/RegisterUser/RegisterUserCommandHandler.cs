using AutoMapper;
using MediatR;
using Notes.Application.Common.Exceptions;
using Notes.Application.Common.Helpers;
using Notes.Application.Interfaces;
using Notes.Application.Users.Dto;
using Notes.Domain;

namespace Notes.Application.Users.Commands.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserDto>
	{
        readonly UsersHelper _usersHelper;
        readonly IMapper _mapper;
        readonly IUsersContext _usersContext;

        public RegisterUserCommandHandler(UsersHelper usersHelper, IUsersContext usersContext, IMapper mapper)
		{
			_usersHelper = usersHelper;
			_usersContext = usersContext;
			_mapper = mapper;
		}

		public async Task<UserDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
		{
			if (UserIsExistsByLogin(request.Login))
				throw new UserIsAlreadyRegisteredException("User with this login has already been registred");

			var savededUserAndToken = await CreateNewUserAndSave(request, cancellationToken);
			return MapToDto(savededUserAndToken);
		}

		bool UserIsExistsByLogin(string login)
		{
			User? selectedUser = _usersContext.Users.FirstOrDefault(u => u.Login == login);
			return selectedUser != null;
		}

		async Task<(string accessToken, User user)> CreateNewUserAndSave
			(RegisterUserCommand request, CancellationToken cancellationToken)
		{
			User newUser = new User() { Login = request.Login, Password = request.Password };
			User createdUser = await _usersHelper.SaveUserAsync(newUser, cancellationToken);

			var tokens = await CreateAndSaveTokensForUserAsync(createdUser);
			return (tokens.accessToken, createdUser);
		}

		async Task<(string accessToken, string refreshToken)> CreateAndSaveTokensForUserAsync(User createdUser)
		{
			var tokens = _usersHelper.CreateTokens(createdUser.Id);
			await SaveRefreshTokenForUserAsync(createdUser, tokens.refreshToken);
			return tokens;
		}

		async Task SaveRefreshTokenForUserAsync(User user, string refreshToken)
		{
			user.RefreshToken = refreshToken;
			await _usersHelper.SaveUserAsync(user, CancellationToken.None);
		}

		UserDto MapToDto((string accessToken, User createdUser) savedUserAndTokens)
		{
			UserDto addedUser = _mapper.Map<UserDto>(savedUserAndTokens.createdUser);
			addedUser.AccessToken = savedUserAndTokens.accessToken;
			return addedUser;
		}
	}
}
