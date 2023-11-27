using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("image")]
    public class Image
    {
        [Column("ImageId")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Path to a picture is required")]
        [StringLength(260, ErrorMessage = "Path to a picture can't be longer than 260 characters")]
        public string? Path { get; set; }

        [ForeignKey(nameof(Room))]
        public Guid RoomId { get; set; }
        public Room? Room { get; set; }
    }
}
