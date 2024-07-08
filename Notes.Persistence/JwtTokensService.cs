using Microsoft.IdentityModel.Tokens;
using Notes.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Notes.Application.Common.Exceptions;

namespace Notes.Persistence
{
    public class JwtTokensService : IJwtTokensService
    {
        const string _idPropertyName = "jti";
        JwtSecurityTokenHandler _jwtHeandler;

        readonly string _issuer, _audience;
        readonly int _accessTokenLiveTimeSeconds, _refreshTokenLiveTimeSeconds;
        readonly SymmetricSecurityKey _secret;

        public JwtTokensService(string secret, string issuer, string audience, int accessTokenLiveTimeSeconds, int refreshTokenLiveTimeSeconds)
        {
            _issuer = issuer;
            _audience = audience;
            _accessTokenLiveTimeSeconds = accessTokenLiveTimeSeconds;
            _refreshTokenLiveTimeSeconds = refreshTokenLiveTimeSeconds;
            _secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            _jwtHeandler = new JwtSecurityTokenHandler();
        }

        public string GenerateAccessToken(int userId)
        {
            return CreateToken(userId, _accessTokenLiveTimeSeconds);
        }

        public string GenerateRefrechToken(int userId)
        {
            return CreateToken(1, _refreshTokenLiveTimeSeconds);
        }

        public string CreateToken(int userId, int liveTimeSeconds)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(_idPropertyName, userId.ToString())
             };

            string token = _jwtHeandler.WriteToken(new JwtSecurityToken(
                issuer: _issuer, // издатель
                audience: _audience, // поставщик
                claims: claims,
                expires: DateTime.UtcNow.AddSeconds(liveTimeSeconds),
                signingCredentials: new SigningCredentials(_secret,
                    SecurityAlgorithms.HmacSha256
            )));

            return token;
        }

       

        public int GetUserIdFromToken(string jwtToken)
        {
            if (!TokenIsValid(jwtToken))
                throw new ValidationException("JWT token is not valid");

            var parsedToken = _jwtHeandler.ReadJwtToken(jwtToken);
            var userId = int.Parse(parsedToken.Claims.First(c => c.Type == _idPropertyName).Value);

            return userId;
        }

        public bool TokenIsValid(string jwtToken)
        {
            try
            {
                return ValidateToken(jwtToken);
            }
            catch (SecurityTokenInvalidAudienceException)
            {
                return false;
            }
        }

        bool ValidateToken(string jwtToken)
        {
            _jwtHeandler.ValidateToken(jwtToken,
                           new TokenValidationParameters()
                           {
                               ClockSkew = TimeSpan.FromSeconds(1),
                               // секрет
                               IssuerSigningKey = _secret,
                               ValidateIssuerSigningKey = true,
                               // время жизни
                               ValidateLifetime = true,
                               // потребитель
                               ValidAudience = _audience,
                               ValidateAudience = true,
                               // издатель
                               ValidIssuer = _issuer,
                               ValidateIssuer = true

                           }, out _);

            return true;
        }
    }
}
