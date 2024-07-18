using System.ComponentModel.DataAnnotations;

namespace Notes.WebApi.Models.Users
{
    public class LoginRequest
    {
        [Required]
        public string Login { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
