using Contracts;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class RoomRepository : RepositoryBase<Room>, IRoomRepository
    {
        public RoomRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public IEnumerable<Room> GetAllRooms()
        {
            return FindAll()
                .OrderBy(room => room.Name)
                .ToList();
        }

        public Room GetRoomById(Guid roomId)
        {
            return FindByCondition(room => room.Id.Equals(roomId))
                .FirstOrDefault();
        }

        public Room GetRoomWithDetails(Guid roomId)
        {
            return FindByCondition(room => room.Id.Equals(roomId))
                .Include(room => room.Reservations)
                .FirstOrDefault();
        }
    }
}
