namespace Notes.Application.Interfaces
{
    public interface IJwtTokensService
    {
        public string GenerateAccessToken(int userId);

        public string GenerateRefrechToken(int userId);

        public int GetUserIdFromToken(string jwtToken);

        public bool TokenIsValid(string jwtToken);
    }
}
