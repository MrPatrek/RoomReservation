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

        public async Task<IEnumerable<Room>> GetAllRoomsAsync()
        {
            return await FindAll()
                .OrderBy(room => room.Name)
                .ToListAsync();
        }

        public async Task<Room> GetRoomByIdAsync(Guid roomId)
        {
            return await FindByCondition(room => room.Id.Equals(roomId))
                .FirstOrDefaultAsync();
        }

        public async Task<Room> GetRoomWithReservationsAsync(Guid roomId)
        {
            return await FindByCondition(room => room.Id.Equals(roomId))
                .Include(room => room.Reservations)
                .FirstOrDefaultAsync();
        }
        
        public async Task<Room> GetRoomWithImagesAsync(Guid roomId)
        {
            return await FindByCondition(room => room.Id.Equals(roomId))
                .Include(room => room.Images)
                .FirstOrDefaultAsync();
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



        public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateOnly arrival, DateOnly departure)
        {
            return await FindAll()
                .Include(room => room.Reservations)
                .Include(room => room.Images)
                .Where(room => room.Reservations
                    .All(reservation => reservation.Departure <= arrival || reservation.Arrival >= departure))
                .OrderBy(room => room.Name)
                .ToListAsync();
        }
    }
}
