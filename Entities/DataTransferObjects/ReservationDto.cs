namespace Entities.DataTransferObjects
{
    public class ReservationDto
    {
        public Guid Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateOnly Arrival { get; set; }
        public DateOnly Departure { get; set; }
        public string? GuestName { get; set; }
        public string? GuestEmail { get; set; }
        public string? GuestTel { get; set; }
        public string? Remark { get; set; }
        public decimal Price { get; set; }
        public RoomDto? Room { get; set; }
    }
}
