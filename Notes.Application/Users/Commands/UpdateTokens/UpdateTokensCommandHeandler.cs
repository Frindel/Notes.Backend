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
        ITokensGenerator _tokensGenerator;

        public UpdateTokensCommandHeandler(IUsersContext users, ITokensGenerator tokensGenerator)
        {
            _usersContext = users;
            _tokensGenerator = tokensGenerator;
        }

        public async Task<TokensDto> Handle(UpdateTokensCommand request, CancellationToken cancellationToken)
        {
            User? user = GetUserByRefreshToken(request.RefreshToken);
            if (user == null)
                throw new UserNotFoundException("user with this refresh-token was not found");

            var newTokens = await UpdateAndSaveTokens(user, cancellationToken);
            return newTokens;
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
                AssessToken = _tokensGenerator.GenerateAccessToken(user.Id),
                RefreshToken = _tokensGenerator.GenerateRefrechToken()
            };
        }

        async Task SaveRefreshToken(string refreshToken, User user, CancellationToken cancellationToken)
        {
            user.RefreshToken = refreshToken;
            await _usersContext.SaveChangesAsync(cancellationToken);
        }

    }
}
