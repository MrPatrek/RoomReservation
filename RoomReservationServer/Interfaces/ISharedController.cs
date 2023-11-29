using Microsoft.AspNetCore.Mvc;

namespace RoomReservationServer.Interfaces
{
    public interface ISharedController
    {
        Task<IActionResult> IsRoomAvailable(Guid id, DateOnly arrival, DateOnly departure);
        IActionResult CheckDates(DateOnly arrival, DateOnly departure);
        Task<IActionResult> DeleteImagesForRoom(Guid roomId);
    }
}
