using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("room")]
    public class Room
    {
        public Guid RoomId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name can't be longer than 50 characters")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [DataType(DataType.Currency)]
        [Range(typeof(decimal), "0", "999999.99m")]         // m for decimal
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Short description is required")]
        [StringLength(100, ErrorMessage = "Short description cannot be longer than 100 characters")]
        public string? DescriptionShort { get; set; }

        [StringLength(500, ErrorMessage = "Long description cannot be longer than 500 characters")]
        public string? DescriptionLong { get; set; }

        public ICollection<Reservation>? Reservations { get; set; }
    }
}
