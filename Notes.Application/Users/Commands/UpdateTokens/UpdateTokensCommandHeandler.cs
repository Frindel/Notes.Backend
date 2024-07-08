using MediatR;
using Notes.Application.Common.Exceptions;
using Notes.Application.Interfaces;
using Notes.Application.Users.Dto;
using Notes.Domain;

namespace Notes.Application.Users.Commands.UpdateTokens
{
    public class UpdateTokensCommandHeandler : IRequestHandler<UpdateTokensCommand, TokensDto>
    {
        IUsersContext _usersContext;
        IJwtTokensService _jwtTokens;

        public UpdateTokensCommandHeandler(IUsersContext users, IJwtTokensService jwtTokens)
        {
            _usersContext = users;
            _jwtTokens = jwtTokens;
        }

        public async Task<TokensDto> Handle(UpdateTokensCommand request, CancellationToken cancellationToken)
        {
            if (!RefreshTokenIsValid(request.RefreshToken))
                throw new ValidationException("refresh token is not valid.");

            User? user = GetUserByRefreshToken(request.RefreshToken);
            if (user == null)
                throw new UserNotFoundException("user with this refresh-token was not found");

            var newTokens = await UpdateAndSaveTokens(user, cancellationToken);
            return newTokens;
        }

        bool RefreshTokenIsValid(string refreshToken)
        {
            return _jwtTokens.TokenIsValid(refreshToken);
        }

        User? GetUserByRefreshToken(string refreshToken)
        {
            return _usersContext.Users.FirstOrDefault(u => u.RefreshToken == refreshToken);
        }

        async Task<TokensDto> UpdateAndSaveTokens(User user, CancellationToken cancellationToken)
        {
            var tokens = CreateTokens(user);
            await SaveRefreshToken(tokens.RefreshToken, user, cancellationToken);

            return tokens;
        }

        TokensDto CreateTokens(User user)
        {
            return new TokensDto()
            {
                AssessToken = _jwtTokens.GenerateAccessToken(user.Id),
                RefreshToken = _jwtTokens.GenerateRefrechToken(user.Id)
            };
        }

        async Task SaveRefreshToken(string refreshToken, User user, CancellationToken cancellationToken)
        {
            user.RefreshToken = refreshToken;
            await _usersContext.SaveChangesAsync(cancellationToken);
        }

    }
}
