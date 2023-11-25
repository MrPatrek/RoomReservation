using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;

namespace RoomReservationServer.Controllers
{
    [Route("api/reservation")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repository;
        private IMapper _mapper;

        public ReservationController(ILoggerManager logger, IRepositoryWrapper repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
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
    }
}
