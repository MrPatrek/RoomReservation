﻿using Entities.Models;

namespace Contracts
{
    public interface IRoomRepository
    {
        IEnumerable<Room> GetAllRooms();
        Room GetRoomById(Guid roomId);
        Room GetRoomWithReservations(Guid roomId);
        Room GetRoomWithImages(Guid roomId);
        void CreateRoom(Room room);
        void UpdateRoom(Room room);
        void DeleteRoom(Room room);

        public IEnumerable<Room> GetAvailableRooms(DateOnly arrival, DateOnly departure);
    }
}
