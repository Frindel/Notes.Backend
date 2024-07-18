using MediatR;
using Notes.Application.Common.Exceptions;
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

        public UpdateTokensCommandHandler(UsersHelper usersHelper, IUsersContext users, IJwtTokensService jwtTokens)
        {
            _usersHelper = usersHelper;
            _usersContext = users;
            _jwtTokens = jwtTokens;
        }

        public async Task<TokensDto> Handle(UpdateTokensCommand request, CancellationToken cancellationToken)
        {
            if (!_jwtTokens.TokenIsValid(request.RefreshToken))
                throw new ValidationException("refresh token is not valid.");

            User user = await _usersHelper.GetUserByIdAsync(request.UserId);
            var newTokens = await UpdateAndSaveTokens(user, cancellationToken);
            return MapToDto(newTokens);
        }

        async Task<(string accessToken, string refreshToken)> UpdateAndSaveTokens(User user, CancellationToken cancellationToken)
        {
            var tokens = _usersHelper.CreateTokens(user.Id);
            await SaveRefreshToken(tokens.refreshToken, user, cancellationToken);
            return tokens;
        }

        async Task SaveRefreshToken(string refreshToken, User user, CancellationToken cancellationToken)
        {
            user.RefreshToken = refreshToken;
            await _usersContext.SaveChangesAsync(cancellationToken);
        }

        TokensDto MapToDto((string accessToken, string refreshToken) tokens) =>
            new TokensDto() { AssessToken = tokens.accessToken, RefreshToken = tokens.refreshToken };

    }
}
