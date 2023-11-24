using System.ComponentModel.DataAnnotations;

namespace Entities.DataTransferObjects
{
    public class RoomForCreationDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name can't be longer than 50 characters")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [DataType(DataType.Currency)]
        [Range(typeof(decimal), "0", "999999,99")]      // despite it being decimal, "m" in the end was causing an exception + comma needed instead of period
        // https://stackoverflow.com/a/32068185
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Short description is required")]
        [StringLength(100, ErrorMessage = "Short description cannot be longer than 100 characters")]
        public string? DescriptionShort { get; set; }

        [StringLength(500, ErrorMessage = "Long description cannot be longer than 500 characters")]
        public string? DescriptionLong { get; set; }
    }
}
