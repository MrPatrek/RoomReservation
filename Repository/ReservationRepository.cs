using Contracts;
using Entities;
using Entities.Models;

namespace Repository
{
    public class ReservationRepository : RepositoryBase<Reservation>, IReservationRepository
    {
        public ReservationRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }
}
