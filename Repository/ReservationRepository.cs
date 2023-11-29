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

        public async Task<IEnumerable<Reservation>> GetAllReservationsAsync()
        {
            return await FindAll()
                .OrderBy(reservation => reservation.DateCreated)
                .ToListAsync();
        }

        public async Task<Reservation> GetReservationByIdAsync(Guid reservationId)
        {
            return await FindByCondition(reservation => reservation.Id.Equals(reservationId))
                .FirstOrDefaultAsync();
        }

        public async Task<Reservation> GetReservationWithDetailsAsync(Guid reservationId)
        {
            return await FindByCondition(reservation => reservation.Id.Equals(reservationId))
                .Include(reservation => reservation.Room)
                .FirstOrDefaultAsync();
        }

        // this one is used by DeleteRoom action menthod
        public async Task<IEnumerable<Reservation>> GetReservationsForRoomAsync(Guid roomId)
        {
            return await FindByCondition(reservation => reservation.RoomId.Equals(roomId))
                .ToListAsync();
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
