﻿using AutoMapper;
using Contracts;
using EmailService;
using EmailService.Interfaces;
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
        private IEmailSender _emailSender;

        public ReservationController(ILoggerManager logger, IRepositoryWrapper repository, IMapper mapper, ISharedController sharedController, IEmailSender emailSender)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
            _sharedController = sharedController;
            _emailSender = emailSender;
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

                // Here we should have had the guest email from the reservation, but we need to be sure it works, so that's why here we have email of mine.
                var messageToGuest = new Message(new string[] { "oleksbab20@gmail.com" }, $"Your booking confirmation, ID: {createdReservation.Id}", @$"
                    <p>Dear <i>{createdReservation.GuestName}</i>,
                    <br>Thank you for choosing us. Your booking details are the following:</p>

                    <p>Reservation ID: <i>{createdReservation.Id}</i>,
                    <br>Your room: <i>{room.Name}</i>,
                    <br>Arrival date: {createdReservation.Arrival},
                    <br>Departure date: {createdReservation.Departure},
                    <br>Price per night: {room.Price} EUR,
                    <br>Total price: <b>{createdReservation.Price} EUR</b>.</p>

                    <p>Best regards,
                    <br>Room Reservations</p>
");
                _emailSender.SendEmail(messageToGuest);

                var messageToHotel = new Message(new string[] { "oleksbab20@gmail.com" }, $"New reservation, ID: {createdReservation.Id}", @$"
                    <p>New reservation details are the following:</p>

                    <p>Reservation ID: <i>{createdReservation.Id}</i>,
                    <br>Creation date: {createdReservation.DateCreated},
                    <br>Room: <i>{room.Name}</i> (ID: <i>{room.Id}</i>),
                    <br>Arrival date: {createdReservation.Arrival},
                    <br>Departure date: {createdReservation.Departure},
                    <br>Price per night: {room.Price} EUR,
                    <br>Total price: <b>{createdReservation.Price} EUR</b>.</p>

                    <p>Guest name: {createdReservation.GuestName},
                    <br>Guest email: {createdReservation.GuestEmail},
                    <br>Guest phone number: {createdReservation.GuestTel},
                    <br>Remark: {(createdReservation.Remark is not null ? createdReservation.Remark : "<i>blank</i>")}.</p>
");
                _emailSender.SendEmail(messageToHotel);

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

                var reservationEntity = _repository.Reservation.GetReservationWithDetails(id);          // with details so that we can then get the room data for the part below
                if (reservationEntity is null)
                {
                    _logger.LogError($"Reservation with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                // once all checks are complete, proceed with the request

                _mapper.Map(reservation, reservationEntity);

                _repository.Reservation.UpdateReservation(reservationEntity);
                _repository.Save();

                var messageToGuest = new Message(new string[] { "oleksbab20@gmail.com" }, $"Update for your booking guest details, ID: {reservationEntity.Id}", @$"
                    <p>Dear <i>{reservationEntity.GuestName}</i>,
                    <br>Your <b>guest</b> details have been updated. Your new guest details now look as follows:</p>

                    <p>Guest name: {reservationEntity.GuestName},
                    <br>Guest email: {reservationEntity.GuestEmail},
                    <br>Guest phone number: {reservationEntity.GuestTel},
                    <br>Remark: {(reservationEntity.Remark is not null ? reservationEntity.Remark : "<i>blank</i>")}.</p>

                    <p>Just to remind you, here is your original booking details:
                    <br>Reservation ID: <i>{reservationEntity.Id}</i>,
                    <br>Your room: <i>{reservationEntity.Room.Name}</i>,
                    <br>Arrival date: {reservationEntity.Arrival},
                    <br>Departure date: {reservationEntity.Departure},
                    <br>Price per night: {reservationEntity.Room.Price} EUR,
                    <br>Total price: <b>{reservationEntity.Price} EUR</b>.</p>

                    <p>Best regards,
                    <br>Room Reservations</p>
");
                _emailSender.SendEmail(messageToGuest);

                var messageToHotel = new Message(new string[] { "oleksbab20@gmail.com" }, $"UPDATED reservation guest details, ID: {reservationEntity.Id}", @$"
                    <p>Reservation <b>guest</b> details have been updated. Guest details now look as follows:</p>

                    <p>Guest name: {reservationEntity.GuestName},
                    <br>Guest email: {reservationEntity.GuestEmail},
                    <br>Guest phone number: {reservationEntity.GuestTel},
                    <br>Remark: {(reservationEntity.Remark is not null ? reservationEntity.Remark : "<i>blank</i>")}.</p>

                    <p>Other reservation details:
                    <br>Reservation ID: <i>{reservationEntity.Id}</i>,
                    <br>Creation date: {reservationEntity.DateCreated},
                    <br>Room: <i>{reservationEntity.Room.Name}</i> (ID: <i>{reservationEntity.Room.Id}</i>),
                    <br>Arrival date: {reservationEntity.Arrival},
                    <br>Departure date: {reservationEntity.Departure},
                    <br>Price per night: {reservationEntity.Room.Price} EUR,
                    <br>Total price: <b>{reservationEntity.Price} EUR</b>.</p>
");
                _emailSender.SendEmail(messageToHotel);

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

                var messageToGuest = new Message(new string[] { "oleksbab20@gmail.com" }, $"Your booking is removed, ID: {reservation.Id}", @$"
                    <p>Dear <i>{reservation.GuestName}</i>,
                    <br>Your booking has been removed (reservation ID: <i>{reservation.Id}</i>, already an old one).</p>

                    <p>Best regards,
                    <br>Room Reservations</p>
");
                _emailSender.SendEmail(messageToGuest);

                var messageToHotel = new Message(new string[] { "oleksbab20@gmail.com" }, $"REMOVED reservation, ID: {reservation.Id}", @$"
                    <p>A reservation has been removed (reservation ID: <i>{reservation.Id}</i>, already an old one).</p>
");
                _emailSender.SendEmail(messageToHotel);

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
