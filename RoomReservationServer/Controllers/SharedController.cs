﻿using Contracts;
using Microsoft.AspNetCore.Mvc;
using RoomReservationServer.Interfaces;

namespace RoomReservationServer.Controllers
{
    public class SharedController : ControllerBase, ISharedController
    {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryWrapper _repository;

        public SharedController(ILoggerManager logger, IRepositoryWrapper repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<IActionResult> IsRoomAvailableAsync(Guid id, DateOnly arrival, DateOnly departure)
        {
            // check if dates follow the common sense
            IActionResult check = CheckDates(arrival, departure);
            if (check is BadRequestObjectResult)
            {
                return check;
            }

            // if all is ok, proceed with the request:
            var room = await _repository.Room.GetRoomWithReservationsAsync(id);
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
                return Ok(false);
            }
            else
            {
                _logger.LogInfo($"Room with id: {id}, is available between {arrival} and {departure} dates.");
                return Ok(true);
            }
        }

        public IActionResult CheckDates(DateOnly arrival, DateOnly departure)
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

        public async Task<IActionResult> DeleteImagesForRoomAsync(Guid roomId)
        {
            var images = await _repository.Image.GetImagesForRoomAsync(roomId);
            if (!images.Any())
            {
                _logger.LogInfo($"No images to delete for the room with ID: {roomId}");
                return NoContent();
            }

            // first, check
            foreach (var image in images)
            {
                if (!System.IO.File.Exists(image.Path))
                {
                    _logger.LogError($"Image with id: {image.Id}, HAS been found in the DB, but not in the resources... None of images was removed.");
                    return NotFound($"Image with id: {image.Id}, HAS been found in the DB, but not in the resources... None of images was removed.");
                }
            }

            // then, delete
            foreach (var image in images)
            {
                System.IO.File.Delete(image.Path);
                _repository.Image.DeleteImage(image);
            }

            await _repository.SaveAsync();

            return NoContent();
        }
    }
}
