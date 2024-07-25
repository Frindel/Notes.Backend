using MediatR;
using Notes.Application.Common.Exceptions;
using Notes.Application.Common.Helpers;
using Notes.Application.Users.Dto;
using Notes.Domain;

namespace Notes.Application.Users.Commands.LoginUser
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, TokensDto>
    {
        readonly UsersHelper _usersHelper;

        private CancellationToken _cancellationToken = CancellationToken.None;

        public LoginUserCommandHandler(UsersHelper usersHelper)
        {
            _usersHelper = usersHelper;
        }

        public async Task<TokensDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            User loginedUser = await _usersHelper.GetUserByLoginAsync(request.Login);
            if (!IsLoginAndPasswordValid(request, loginedUser))
                throw new InvalidLoginOrPasswordException("invalid user login or password");

            var tokens = await CreateAndSaveTokensAsync(loginedUser);
            return MapToDto(tokens);
        }

        bool IsLoginAndPasswordValid(LoginUserCommand request, User user)
        {
            return request.Login == user.Login && request.Password == user.Password;
        }

        async Task<(string accessToken, string refreshToken)> CreateAndSaveTokensAsync(User user)
        {
            var tokens = _usersHelper.CreateTokens(user.Id);
            await SaveRefreshTokenAsync(tokens.refreshToken, user);
            return tokens;
        }

        async Task SaveRefreshTokenAsync(string refreshToken, User user)
        {
            user.RefreshToken = refreshToken;
            await _usersHelper.SaveUserAsync(user, _cancellationToken);
        }

        TokensDto MapToDto((string accessToken, string refreshToken) tokens) =>
            new TokensDto()
            {
                AssessToken = tokens.accessToken,
                RefreshToken = tokens.refreshToken
            };
    }
}
