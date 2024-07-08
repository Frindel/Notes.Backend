using MediatR;
using Microsoft.EntityFrameworkCore;
using Notes.Application.Common.Exceptions;
using Notes.Application.Interfaces;
using Notes.Application.Users.Dto;
using Notes.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.Application.Users.Commands.LoginUser
{
    public class LoginUserCommandHeandler : IRequestHandler<LoginUserCommand, TokensDto>
    {
        IUsersContext _users;
        IJwtTokensService _jwtTokens;

        public LoginUserCommandHeandler(IUsersContext users, IJwtTokensService jwtTokens) 
        {
            _users = users;
            _jwtTokens = jwtTokens;
        }

        public async Task<TokensDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            User? user = await GetUserByLogin(request.Login);
            if (user == null)
                throw new UserNotFoundException("user does not exist");

            if (!LoginAndPasswordIsValid(request, user))
                throw new InvalidLoginOrPasswordException("invalid user login or password");

            var tokens = await CreateAndSaveTokensAsync(user, cancellationToken);
            return tokens;
        }

        Task<User?> GetUserByLogin(string login)
        {
            return _users.Users.FirstOrDefaultAsync(u=>u.Login == login);
        }

        bool LoginAndPasswordIsValid(LoginUserCommand request, User user)
        {
            return request.Login == user.Login && request.Password == user.Password;
        }

        async Task<TokensDto> CreateAndSaveTokensAsync(User user, CancellationToken token)
        {
            var tokens = CreateTokens(user);
            await SaveRefreshTokenAsync(tokens.RefreshToken, user, token);

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

        async Task SaveRefreshTokenAsync(string refreshToken, User user, CancellationToken token)
        {
            user.RefreshToken = refreshToken;
            await _users.SaveChangesAsync(token);
        }
    }
}
