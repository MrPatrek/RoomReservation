using Entities.Models;

namespace Contracts
{
    public interface IReservationRepository
    {
        IEnumerable<Reservation> ReservationsForRoom(Guid roomId);
    }
}
