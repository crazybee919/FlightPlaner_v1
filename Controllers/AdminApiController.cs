using FlightPlaner.Models;
using FlightPlaner.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightPlaner.Controllers
{
    [Authorize]
    [Route("admin-api")]
    [ApiController]
    public class AdminApiController : ControllerBase
    {
        private readonly FlightPlannerDBContext _context;

        public AdminApiController(FlightPlannerDBContext context)
        {
            _context = context;
        }
        private static readonly object lockObject = new object();

        [HttpDelete]
        [Route("flights/{id}")]
        public IActionResult DeleteFlight(int id)
        {
            lock (lockObject)
            {
                var flight = _context.Flights.Include(flight => flight.To)
                                             .Include(flight => flight.From).SingleOrDefault(flight => flight.Id == id);
                if (flight == null)
                {
                    return Ok();
                }

                _context.Flights.Remove(flight);
                _context.SaveChanges();

                return Ok();
            }
        }

        [HttpGet]
        [Route("flights/{id}")]
        public IActionResult GetFlight(int id)
        {
            lock (lockObject)
            {
                var flight = _context.Flights.FirstOrDefault(x => x.Id == id);
                if (flight == null)
                {
                    return NotFound();
                }
                _context.SaveChanges();

                return Ok(flight);
            }
        }

        [HttpPut]
        [Route("flights")]
        public IActionResult AddFlight(Flight flight)
        {
            lock (lockObject)
            {
                if (!FlightStorage.IsFlightTimeCorrect(flight) || FlightStorage.AreAirportsSame(flight) ||
                    FlightStorage.IsFlightValuesEmpty(flight))
                {
                    return BadRequest();
                }
                
                if (_context.Flights.Any(x=> x.ArrivalTime == flight.ArrivalTime && x.DepartureTime == flight.DepartureTime && x.From.AirportCode == flight.From.AirportCode && x.To.AirportCode == flight.To.AirportCode && x.Carrier == flight.Carrier))
                {
                    return Conflict();
                }

                _context.Flights.Add(flight);
                _context.SaveChanges();

                return Created("", flight);
            }
        }
    }
}