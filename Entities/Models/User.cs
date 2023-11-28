using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("user")]
    public class User
    {
        [Column("UserId")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(40, ErrorMessage = "Username can't be longer than 40 characters")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password hash is required")]
        [StringLength(44, ErrorMessage = "Password hash can't be longer than 44 characters")]
        public string? PasswordHash { get; set; }

        [Required(ErrorMessage = "Password salt is required")]
        [StringLength(24, ErrorMessage = "Password salt can't be longer than 24 characters")]
        public string? PasswordSalt { get; set; }
    }
}
