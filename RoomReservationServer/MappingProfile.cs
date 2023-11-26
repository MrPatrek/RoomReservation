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

            CreateMap<RoomForCreationDto, Room>();

            CreateMap<RoomForUpdateDto, Room>();

            CreateMap<Room, RoomAvailableDto>();

            CreateMap<Reservation, ReservationDto>();

            CreateMap<ReservationForCreationDto, Reservation>();

            CreateMap<ReservationForUpdateDto, Reservation>();
        }
    }
}
