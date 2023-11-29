using Entities.Models;

namespace Contracts
{
    public interface IRoomRepository
    {
        Task<IEnumerable<Room>> GetAllRoomsAsync();
        Task<Room> GetRoomByIdAsync(Guid roomId);
        Task<Room> GetRoomWithReservationsAsync(Guid roomId);
        Task<Room> GetRoomWithImagesAsync(Guid roomId);
        void CreateRoom(Room room);
        void UpdateRoom(Room room);
        void DeleteRoom(Room room);

        Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateOnly arrival, DateOnly departure);
    }
}
