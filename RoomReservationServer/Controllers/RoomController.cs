using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
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

        [HttpGet("{id}", Name = "RoomById")]
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

        [HttpPost]
        public IActionResult CreateRoom([FromBody] RoomForCreationDto room)
        {
            try
            {
                if (room is null)
                {
                    _logger.LogError("Room object sent from client is null.");
                    return BadRequest("Room object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid room object sent from client.");
                    return BadRequest("Invalid model object");
                }

                var roomEntity = _mapper.Map<Room>(room);

                _repository.Room.CreateRoom(roomEntity);
                _repository.Save();

                var createdRoom = _mapper.Map<RoomDto>(roomEntity);

                return CreatedAtRoute("RoomById", new { id = createdRoom.Id }, createdRoom);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateRoom action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateRoom(Guid id, [FromBody] RoomForUpdateDto room)
        {
            try
            {
                if (room is null)
                {
                    _logger.LogError("Room object sent from client is null.");
                    return BadRequest("Room object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid room object sent from client.");
                    return BadRequest("Invalid model object");
                }

                var roomEntity = _repository.Room.GetRoomById(id);
                if (roomEntity is null)
                {
                    _logger.LogError($"Room with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                _mapper.Map(room, roomEntity);

                _repository.Room.UpdateRoom(roomEntity);
                _repository.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateRoom action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteRoom(Guid id)
        {
            try
            {
                var room = _repository.Room.GetRoomById(id);
                if (room == null)
                {
                    _logger.LogError($"Room with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                if (_repository.Reservation.ReservationsForRoom(id).Any())
                {
                    _logger.LogError($"Cannot delete room with id: {id}. It has related reservations. Delete those reservations first");
                    return BadRequest("Cannot delete room. It has related reservations. Delete those reservations first");
                }

                _repository.Room.DeleteRoom(room);
                _repository.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteRoom action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
