using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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





        // Now, special action methods:



        [HttpGet("availability")]
        public IActionResult GetAvailableRooms(
            [FromQuery(Name = "arrival")][Required(ErrorMessage = "Specify the arrival date.")] DateTime arrivalDt,
            [FromQuery(Name = "departure")][Required(ErrorMessage = "Specify the departure date.")] DateTime departureDt
            )
        {
            try
            {
                DateOnly arrival = DateOnly.FromDateTime(arrivalDt);
                DateOnly departure = DateOnly.FromDateTime(departureDt);

                // check if dates follow the common sense
                IActionResult check = checkDates(arrival, departure);
                if (check is BadRequestObjectResult)
                {
                    return check;
                }

                // if all is ok, proceed with the request:
                var availableRooms = _repository.Room.GetAvailableRooms(arrival, departure);
                if (!availableRooms.Any())
                {
                    _logger.LogInfo($"No rooms between {arrival} and {departure} dates have been found.");
                    return NoContent();
                }
                else
                {
                    _logger.LogInfo($"Returned rooms between {arrival} and {departure} dates.");

                    var availableRoomsResult = _mapper.Map<IEnumerable<RoomAvailableDto>>(availableRooms);

                    var numNights = departure.DayNumber - arrival.DayNumber;
                    foreach (var room in availableRoomsResult)
                    {
                        room.PriceTotal = room.Price * numNights;
                    }

                    return Ok(availableRoomsResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAvailableRooms action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/availability")]
        public IActionResult IsRoomAvailable(
            Guid id,
            [FromQuery(Name = "arrival")][Required(ErrorMessage = "Specify the arrival date.")] DateTime arrivalDt,
            [FromQuery(Name = "departure")][Required(ErrorMessage = "Specify the departure date.")] DateTime departureDt
            )
        {
            try
            {
                DateOnly arrival = DateOnly.FromDateTime(arrivalDt);
                DateOnly departure = DateOnly.FromDateTime(departureDt);

                // check if dates follow the common sense
                IActionResult check = checkDates(arrival, departure);
                if (check is BadRequestObjectResult)
                {
                    return check;
                }

                // if all is ok, proceed with the request:
                var room = _repository.Room.GetRoomWithDetails(id);
                if (room == null)
                {
                    _logger.LogError($"Room with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                bool isAvailable = room.Reservations
                    .All(reservation => reservation.Departure <= arrival || reservation.Arrival >= departure);

                if (!isAvailable)
                {
                    _logger.LogInfo($"Room with id: {id}, is not available between {arrival} and {departure} dates.");
                    return NoContent();
                }
                else
                {
                    _logger.LogInfo($"Room with id: {id}, is available between {arrival} and {departure} dates.");
                    return Ok(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside IsRoomAvailable action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        private IActionResult checkDates(DateOnly arrival, DateOnly departure)
        {
            // check for following common sense:
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (!(arrival < departure))
            {
                _logger.LogError($"Arrival date ({arrival}) is not before the departure date ({departure}), but it has to be.");
                return BadRequest("Your arrival date is not before your departure, but it has to be.");
            }
            if (arrival < today)
            {
                _logger.LogError($"Arrival date ({arrival}) cannot be in the past, it has to be at least today ({today}).");
                return BadRequest("Your arrival date must be at least today. It can be later than today, but not before.");
            }

            return NoContent();
        }
    }
}
