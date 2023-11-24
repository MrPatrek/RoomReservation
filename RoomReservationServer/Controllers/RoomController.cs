using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;

namespace RoomReservationServer.Controllers
{
    [Route("api/room")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repository;
        private IMapper _mapper;

        public RoomController(ILoggerManager logger, IRepositoryWrapper repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAllRooms()
        {
            try
            {
                var rooms = _repository.Room.GetAllRooms();
                _logger.LogInfo($"Returned all rooms from database.");

                var roomsResult = _mapper.Map<IEnumerable<RoomDto>>(rooms);
                return Ok(roomsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAllRooms action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetRoomById(Guid id)
        {
            try
            {
                var room = _repository.Room.GetRoomById(id);

                if (room is null)
                {
                    _logger.LogError($"Room with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned room with id: {id}");

                    var roomResult = _mapper.Map<RoomDto>(room);
                    return Ok(roomResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetRoomById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/reservation")]
        public IActionResult GetRoomWithDetails(Guid id)
        {
            try
            {
                var room = _repository.Room.GetRoomWithDetails(id);

                if (room == null)
                {
                    _logger.LogError($"Room with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned room with details for id: {id}");

                    var roomResult = _mapper.Map<RoomDto>(room);
                    return Ok(roomResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetRoomWithDetails action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
