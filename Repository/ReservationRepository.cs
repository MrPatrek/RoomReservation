using Contracts;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class ReservationRepository : RepositoryBase<Reservation>, IReservationRepository
    {
        public ReservationRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public IEnumerable<Reservation> GetAllReservations()
        {
            return FindAll()
                .OrderBy(reservation => reservation.DateCreated)
                .ToList();
        }

        public Reservation GetReservationById(Guid reservationId)
        {
            return FindByCondition(reservation => reservation.Id.Equals(reservationId))
                .FirstOrDefault();
        }

        public Reservation GetReservationWithDetails(Guid reservationId)
        {
            return FindByCondition(reservation => reservation.Id.Equals(reservationId))
                .Include(reservation => reservation.Room)
                .FirstOrDefault();
        }

        // this one is used by DeleteRoom action menthod
        public IEnumerable<Reservation> ReservationsForRoom(Guid roomId)
        {
            return FindByCondition(reservation => reservation.RoomId.Equals(roomId)).ToList();
        }

        public void CreateReservation(Reservation reservation)
        {
            Create(reservation);
        }

        public void UpdateReservation(Reservation reservation)
        {
            Update(reservation);
        }

        public void DeleteReservation(Reservation reservation)
        {
            Delete(reservation);
        }
    }
}
