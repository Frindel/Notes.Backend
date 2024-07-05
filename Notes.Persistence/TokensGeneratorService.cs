using Microsoft.IdentityModel.Tokens;
using Notes.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Notes.Persistence
{
    public class TokensGenerator : ITokensGenerator
    {
        readonly string _issuer, _audience;
        readonly int _tokenLiveTime;
        readonly SymmetricSecurityKey _secret;

        public TokensGenerator(string secret, string issuer, string audience, int tokenLiveTime)
        {
            _issuer = issuer;
            _audience = audience;
            _tokenLiveTime = tokenLiveTime;
            _secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        }

        public string GenerateAccessToken(int userId)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            string token = tokenHandler.WriteToken(new JwtSecurityToken(
                issuer: _issuer, // издатель
                audience: _audience, // поставщик
                claims: claims,
                expires: DateTime.Now.AddSeconds(_tokenLiveTime),
                signingCredentials: new SigningCredentials(_secret,
                    SecurityAlgorithms.HmacSha256
            )));

            return token;
        }

        public string GenerateRefrechToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            var token = Convert.ToBase64String(randomNumber);
            return token;
        }
    }
}
