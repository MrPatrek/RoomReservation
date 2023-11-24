namespace Entities.DataTransferObjects
{
    public class ReservationDto
    {
        public Guid Id { get; set; }
        public DateOnly DateCreated { get; set; }
        public DateOnly Arrival { get; set; }
        public DateOnly Departure { get; set; }
    }
}
