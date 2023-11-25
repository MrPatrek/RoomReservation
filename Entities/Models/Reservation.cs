using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("reservation")]
    public class Reservation
    {
        [Column("ReservationId")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Date created is required")]
        public DateTime DateCreated { get; set; }

        [Required(ErrorMessage = "Arrival date is required")]
        public DateOnly Arrival { get; set; }

        [Required(ErrorMessage = "Departure date is required")]
        public DateOnly Departure { get; set; }

        [Required(ErrorMessage = "Guest name is required")]
        [StringLength(50, ErrorMessage = "Guest name can't be longer than 50 characters")]
        public string? GuestName { get; set; }

        [Required(ErrorMessage = "Guest email is required")]
        [StringLength(319, ErrorMessage = "Guest email can't be longer than 319 characters")]
        [EmailAddress]
        public string? GuestEmail { get; set; }

        [Required(ErrorMessage = "Guest telephone number is required")]
        [StringLength(40, ErrorMessage = "Guest telephone number can't be longer than 40 characters")]
        [Phone]
        public string? GuestTel { get; set; }

        [StringLength(200, ErrorMessage = "Remark can't be longer than 200 characters")]
        public string? Remark { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [DataType(DataType.Currency)]
        [Range(typeof(decimal), "0", "999999999,99")]
        public decimal Price { get; set; }

        [ForeignKey(nameof(Room))]
        public Guid RoomId { get; set; }
        public Room? Room { get; set; }
    }
}
