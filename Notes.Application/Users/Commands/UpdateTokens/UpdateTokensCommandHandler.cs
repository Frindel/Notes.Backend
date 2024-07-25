using FluentValidation;
using MediatR;
using Notes.Application.Common.Helpers;
using Notes.Application.Interfaces;
using Notes.Application.Users.Dto;
using Notes.Domain;

namespace Notes.Application.Users.Commands.UpdateTokens
{
    public class UpdateTokensCommandHandler : IRequestHandler<UpdateTokensCommand, TokensDto>
    {
        readonly UsersHelper _usersHelper;
        readonly IUsersContext _usersContext;
        readonly IJwtTokensService _jwtTokens;

        private CancellationToken _cancellationToken = CancellationToken.None;

        public UpdateTokensCommandHandler(UsersHelper usersHelper, IUsersContext users, IJwtTokensService jwtTokens)
        {
            _usersHelper = usersHelper;
            _usersContext = users;
            _jwtTokens = jwtTokens;
        }

        public async Task<TokensDto> Handle(UpdateTokensCommand request, CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            if (!_jwtTokens.TokenIsValid(request.RefreshToken))
                throw new ValidationException("refresh token is not valid.");

            User user = await _usersHelper.GetUserByIdAsync(request.UserId);
            var newTokens = await UpdateAndSaveTokens(user);
            return MapToDto(newTokens);
        }

        async Task<(string accessToken, string refreshToken)> UpdateAndSaveTokens(User user)
        {
            var tokens = _usersHelper.CreateTokens(user.Id);
            await SaveRefreshToken(tokens.refreshToken, user);
            return tokens;
        }

        async Task SaveRefreshToken(string refreshToken, User user)
        {
            user.RefreshToken = refreshToken;
            await _usersContext.SaveChangesAsync(_cancellationToken);
        }

        TokensDto MapToDto((string accessToken, string refreshToken) tokens) =>
            new TokensDto() { AssessToken = tokens.accessToken, RefreshToken = tokens.refreshToken };
    }
}