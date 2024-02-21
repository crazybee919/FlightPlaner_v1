using FlightPlaner.Models;
using FlightPlaner.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FlightPlaner.Controllers 
{
    [Authorize]
    [Route("admin-api")]
    [ApiController]
    public class AdminApiController : ControllerBase
    {
        private static readonly object lockObject = new object();

        [HttpDelete]
        [Route("flights/{id}")]
        public IActionResult DeleteFlight(int id)
        {
            lock (lockObject)
            {
                var flight = FlightStorage.GetFlightById(id);
                if (flight == null)
                {
                    return Ok();
                }

                FlightStorage.DeleteFlight(flight);

                return Ok();
            }
        }

        [HttpGet]
        [Route("flights/{id}")]
        public IActionResult GetFlight(int id)
        {
            lock (lockObject)
            {
                var flight = FlightStorage.GetFlightById(id);
                if (flight == null)
                {
                    return NotFound();
                }

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

                if (FlightStorage.DoesFlightExsist(flight))
                {
                    return Conflict();
                }

                FlightStorage.AddFlight(flight);

                return Created("", flight);
            }
        }
    }
}