using Contracts;
using Entities;
using Entities.Models;

namespace Repository
{
    public class RoomRepository : RepositoryBase<Room>, IRoomRepository
    {
        public RoomRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }
}
