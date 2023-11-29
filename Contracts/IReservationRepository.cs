using Entities.Models;

namespace Contracts
{
    public interface IReservationRepository
    {
        Task<IEnumerable<Reservation>> GetAllReservationsAsync();
        Task<Reservation> GetReservationByIdAsync(Guid reservationId);
        Task<Reservation> GetReservationWithDetailsAsync(Guid reservationId);
        Task<IEnumerable<Reservation>> GetReservationsForRoomAsync(Guid roomId);      // this one is used by DeleteRoom action menthod
        void CreateReservation(Reservation reservation);
        void UpdateReservation(Reservation reservation);
        void DeleteReservation(Reservation reservation);
    }
}
