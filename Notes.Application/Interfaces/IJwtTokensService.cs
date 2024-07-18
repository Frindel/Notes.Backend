namespace Notes.Application.Interfaces
{
    public interface IJwtTokensService
    {
        public string GenerateAccessToken(int userId);

        public string GenerateRefrechToken(int userId);

        public bool TokenIsValid(string jwtToken);
    }
}
