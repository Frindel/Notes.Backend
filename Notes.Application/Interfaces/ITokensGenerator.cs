using Notes.Domain;

namespace Notes.Application.Interfaces
{
    public interface ITokensGenerator
    {
        public string GenerateAccessToken(int userId);

        public string GenerateRefrechToken();
    }
}
