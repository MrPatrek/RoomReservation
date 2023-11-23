using Contracts;
using Entities;

namespace Repository
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private RepositoryContext _repoContext;
        private IRoomRepository _room;
        private IReservationRepository _reservation;

        public IRoomRepository Room
        {
            get
            {
                if (_room == null)
                {
                    _room = new RoomRepository(_repoContext);
                }

                return _room;
            }
        }

        public IReservationRepository Reservation
        {
            get
            {
                if (_reservation == null)
                {
                    _reservation = new ReservationRepository(_repoContext);
                }

                return _reservation;
            }
        }

        public RepositoryWrapper(RepositoryContext repositoryContext)
        {
            _repoContext = repositoryContext;
        }

        public void Save()
        {
            _repoContext.SaveChanges();
        }
    }
}
