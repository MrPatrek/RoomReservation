using System.ComponentModel.DataAnnotations;

namespace Entities.DataTransferObjects
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(40, ErrorMessage = "Username can't be longer than 40 characters")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(40, ErrorMessage = "Password can't be longer than 40 characters")]
        public string? Password { get; set; }
    }
}
