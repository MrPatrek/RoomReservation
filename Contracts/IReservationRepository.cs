using Entities.Models;

namespace Contracts
{
    public interface IReservationRepository
    {
        IEnumerable<Reservation> GetAllReservations();
        Reservation GetReservationById(Guid reservationId);
        Reservation GetReservationWithDetails(Guid reservationId);
        IEnumerable<Reservation> ReservationsForRoom(Guid roomId);      // this one is used by DeleteRoom action menthod
        void CreateReservation(Reservation reservation);
        void UpdateReservation(Reservation reservation);
        void DeleteReservation(Reservation reservation);
    }
}
