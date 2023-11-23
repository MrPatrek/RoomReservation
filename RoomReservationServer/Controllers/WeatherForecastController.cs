using Contracts;
using Microsoft.AspNetCore.Mvc;

namespace RoomReservationServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private IRepositoryWrapper _repository;

        public WeatherForecastController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            var arrivals1205 = _repository.Reservation.FindByCondition(x => x.Arrival.Equals(new DateOnly(2023, 12, 05)));
            var rooms = _repository.Room.FindAll();

            return new string[] { "value1", "value2" };
        }
    }
}
