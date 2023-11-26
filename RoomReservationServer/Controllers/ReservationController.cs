using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using RoomReservationServer.Interfaces;

namespace RoomReservationServer.Controllers
{
    [Route("api/reservation")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repository;
        private IMapper _mapper;
        private ISharedController _sharedController;

        public ReservationController(ILoggerManager logger, IRepositoryWrapper repository, IMapper mapper, ISharedController sharedController)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
            _sharedController = sharedController;
        }

        [HttpGet]
        public IActionResult GetAllReservations()
        {
            try
            {
                var reservations = _repository.Reservation.GetAllReservations();
                _logger.LogInfo($"Returned all reservations from database.");

                var reservationsResult = _mapper.Map<IEnumerable<ReservationDto>>(reservations);
                return Ok(reservationsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAllReservations action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}", Name = "ReservationById")]
        public IActionResult GetReservationById(Guid id)
        {
            try
            {
                var reservation = _repository.Reservation.GetReservationById(id);

                if (reservation is null)
                {
                    _logger.LogError($"Reservation with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned reservation with id: {id}");

                    var reservationResult = _mapper.Map<ReservationDto>(reservation);
                    return Ok(reservationResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetReservationById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/room")]
        public IActionResult GetReservationWithDetails(Guid id)
        {
            try
            {
                var reservation = _repository.Reservation.GetReservationWithDetails(id);

                if (reservation == null)
                {
                    _logger.LogError($"Reservation with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned reservation with details for id: {id}");

                    var reservationResult = _mapper.Map<ReservationDto>(reservation);
                    return Ok(reservationResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetReservationWithDetails action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public IActionResult CreateReservation([FromBody] ReservationForCreationDto reservation)
        {
            try
            {
                // first, all the checks
                if (reservation is null)
                {
                    _logger.LogError("Reservation object sent from client is null.");
                    return BadRequest("Reservation object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid reservation object sent from client.");
                    return BadRequest("Invalid model object");
                }

                IActionResult check = _sharedController.IsRoomAvailable(reservation.RoomId, reservation.Arrival, reservation.Departure);
                if (check is not OkObjectResult)
                {
                    return check;
                }

                var room = _repository.Room.GetRoomById(reservation.RoomId);
                if (room is null)
                {
                    _logger.LogError($"Room with id: {reservation.RoomId}, hasn't been found in db.");
                    return NotFound();
                }

                // once all checks are complete, proceed with the request
                var reservationEntity = _mapper.Map<Reservation>(reservation);

                var numNights = reservationEntity.Departure.DayNumber - reservationEntity.Arrival.DayNumber;
                reservationEntity.Price = room.Price * numNights;

                reservationEntity.DateCreated = DateTime.UtcNow;

                _repository.Reservation.CreateReservation(reservationEntity);
                _repository.Save();

                var createdReservation = _mapper.Map<ReservationDto>(reservationEntity);

                return CreatedAtRoute("ReservationById", new { id = createdReservation.Id }, createdReservation);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateReservation action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateReservation(Guid id, [FromBody] ReservationForUpdateDto reservation)
        {
            try
            {
                if (reservation is null)
                {
                    _logger.LogError("Reservation object sent from client is null.");
                    return BadRequest("Reservation object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid reservation object sent from client.");
                    return BadRequest("Invalid model object");
                }

                var reservationEntity = _repository.Reservation.GetReservationById(id);
                if (reservationEntity is null)
                {
                    _logger.LogError($"Reservation with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                // once all checks are complete, proceed with the request

                _mapper.Map(reservation, reservationEntity);

                _repository.Reservation.UpdateReservation(reservationEntity);
                _repository.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateReservation action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteReservation(Guid id)
        {
            try
            {
                var reservation = _repository.Reservation.GetReservationById(id);
                if (reservation == null)
                {
                    _logger.LogError($"Reservation with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                _repository.Reservation.DeleteReservation(reservation);
                _repository.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteReservation action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
