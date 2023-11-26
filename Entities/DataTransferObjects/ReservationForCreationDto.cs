using System.ComponentModel.DataAnnotations;

namespace Entities.DataTransferObjects
{
    public class ReservationForCreationDto
    {
        [Required(ErrorMessage = "Arrival date is required")]
        [DataType(DataType.Date)]
        public DateOnly Arrival { get; set; }

        [Required(ErrorMessage = "Departure date is required")]
        [DataType(DataType.Date)]
        public DateOnly Departure { get; set; }

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

        [Required(ErrorMessage = "Id of room is required")]
        public Guid RoomId { get; set; }
    }
}
