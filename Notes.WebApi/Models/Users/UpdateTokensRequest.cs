using System.ComponentModel.DataAnnotations;

namespace Notes.WebApi.Models.Users
{
    public class UpdateTokensRequest
    {
        [Required]
        public string RefreshToken { get; set; } = null!;
    }
}
