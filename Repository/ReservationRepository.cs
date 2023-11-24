using Contracts;
using Entities;
using Entities.Models;
using System.Security.Principal;

namespace Repository
{
    public class ReservationRepository : RepositoryBase<Reservation>, IReservationRepository
    {
        public ReservationRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public IEnumerable<Reservation> ReservationsForRoom(Guid roomId)
        {
            return FindByCondition(reservation => reservation.RoomId.Equals(roomId)).ToList();
        }
    }
}
