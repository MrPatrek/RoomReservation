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

        public Room GetRoomWithReservations(Guid roomId)
        {
            return FindByCondition(room => room.Id.Equals(roomId))
                .Include(room => room.Reservations)
                .FirstOrDefault();
        }
        
        public Room GetRoomWithImages(Guid roomId)
        {
            return FindByCondition(room => room.Id.Equals(roomId))
                .Include(room => room.Images)
                .FirstOrDefault();
        }

        public void CreateRoom(Room room)
        {
            Create(room);
        }

        public void UpdateRoom(Room room)
        {
            Update(room);
        }

        public void DeleteRoom(Room room)
        {
            Delete(room);
        }





        // Now, special room queries:



        public IEnumerable<Room> GetAvailableRooms(DateOnly arrival, DateOnly departure)
        {
            return FindAll()
                .Include(room => room.Reservations)
                .Include(room => room.Images)
                .Where(room => room.Reservations
                    .All(reservation => reservation.Departure <= arrival || reservation.Arrival >= departure))
                .OrderBy(room => room.Name)
                .ToList();

            //.Where(room => room.Reservations
            //        .Where(reservation => reservation.Arrival >= arrival && reservation.Departure <= departure)
            //    )
        }
    }
}
