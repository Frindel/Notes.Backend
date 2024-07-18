using System.ComponentModel.DataAnnotations;

namespace Notes.WebApi.Models.Users
{
    public class RegisterRequest
    {
        [Required]
        public string Login { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
