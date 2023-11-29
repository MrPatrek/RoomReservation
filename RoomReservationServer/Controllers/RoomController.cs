using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomReservationServer.Interfaces;
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
        private ISharedController _sharedController;

        public RoomController(ILoggerManager logger, IRepositoryWrapper repository, IMapper mapper, ISharedController sharedController)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
            _sharedController = sharedController;
        }

        [HttpGet]
        public IActionResult GetAllRooms()
        {
            try
            {
                var rooms = _repository.Room.GetAllRooms();

                if (!rooms.Any())
                {
                    _logger.LogInfo($"There are no rooms in the database.");
                    return NoContent();
                }
                else
                {
                    _logger.LogInfo($"Returned all rooms from database.");

                    var roomsResult = _mapper.Map<IEnumerable<RoomDto>>(rooms);
                    return Ok(roomsResult);
                }

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

        [HttpGet("{id}/reservation"), Authorize]
        public IActionResult GetRoomWithReservations(Guid id)
        {
            try
            {
                var room = _repository.Room.GetRoomWithReservations(id);

                if (room == null)
                {
                    _logger.LogError($"Room with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned room with reservations for id: {id}");

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

        [HttpGet("{id}/image")]
        public IActionResult GetRoomWithImages(Guid id)
        {
            try
            {
                var room = _repository.Room.GetRoomWithImages(id);

                if (room == null)
                {
                    _logger.LogError($"Room with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned room with images for id: {id}");

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

        [HttpPost, Authorize]
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

        [HttpPut("{id}"), Authorize]
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

        [HttpDelete("{id}"), Authorize]
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

                if (_repository.Reservation.GetReservationsForRoom(id).Any())
                {
                    _logger.LogError($"Cannot delete room with id: {id}. It has related reservations. Delete those reservations first");
                    return BadRequest("Cannot delete room. It has related reservations. Delete those reservations first");
                }

                IActionResult deleteRoomsResult = _sharedController.DeleteImagesForRoom(id);
                if (deleteRoomsResult is not NoContentResult)
                {
                    return deleteRoomsResult;
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
                IActionResult check = _sharedController.CheckDates(arrival, departure);
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

                return _sharedController.IsRoomAvailable(id, arrival, departure);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside IsRoomAvailable action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
