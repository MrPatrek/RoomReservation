using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Models;

namespace RoomReservationServer
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Room, RoomDto>();

            CreateMap<Reservation, ReservationDto>();
        }
    }
}
