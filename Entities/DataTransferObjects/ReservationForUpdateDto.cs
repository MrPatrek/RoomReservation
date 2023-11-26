using System.ComponentModel.DataAnnotations;

namespace Entities.DataTransferObjects
{
    public class ReservationForUpdateDto
    {
        [Required(ErrorMessage = "Guest name is required")]
        [StringLength(50, ErrorMessage = "Guest name can't be longer than 50 characters")]
        public string? GuestName { get; set; }

        [Required(ErrorMessage = "Guest email is required")]
        [StringLength(319, ErrorMessage = "Guest email can't be longer than 319 characters")]
        [EmailAddress]
        public string? GuestEmail { get; set; }

        [Required(ErrorMessage = "Guest phone number is required")]
        [StringLength(40, ErrorMessage = "Guest phone number can't be longer than 40 characters")]
        [Phone]
        public string? GuestTel { get; set; }

        [StringLength(200, ErrorMessage = "Guest remark can't be longer than 200 characters")]
        public string? Remark { get; set; }
    }
}
