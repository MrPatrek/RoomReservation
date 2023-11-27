namespace Entities.DataTransferObjects
{
    public class ImageDto
    {
        public Guid Id { get; set; }
        public string? Path { get; set; }

        public RoomDto? Room { get; set; }
    }
}
