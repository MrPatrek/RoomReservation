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
        public DateOnly DateCreated { get; set; }

        [Required(ErrorMessage = "Arrival date is required")]
        public DateOnly Arrival { get; set; }

        [Required(ErrorMessage = "Departure date is required")]
        public DateOnly Departure { get; set; }

        [ForeignKey(nameof(Room))]
        public Guid RoomId { get; set; }
        public Room? Room { get; set; }
    }
}
