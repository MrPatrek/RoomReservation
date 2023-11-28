namespace Entities.DataTransferObjects
{
    public class ImageDto
    {
        public Guid Id { get; set; }
        public string? Path { get; set; }
        public Guid RoomId { get; set; }
    }
}
