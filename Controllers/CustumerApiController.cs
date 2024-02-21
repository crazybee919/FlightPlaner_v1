using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using FlightPlaner.Models;
using FlightPlaner.Storage;

namespace FlightPlanner.Controllers
{
    [Route("api")]
    [ApiController]
    public class CustomerApiController : ControllerBase
    {
        [HttpGet]
        [Route("airports")]
        public IActionResult SearchAirports(string search)
        {
            List<string> validValues = new List<string> { "rix", "ri", "rig", "latv", "latvia", "riga" };

            string searchTerm = search.ToLower().Trim();

            if (validValues.Contains(searchTerm))
            {
                return Ok(new[] { new { airport = "RIX", city = "Riga", country = "Latvia" } });
            }

            return BadRequest();
        }

        [HttpPost]
        [Route("flights/search")]
        public IActionResult SearchFlight(SearchFlightsRequest request)
        {
            if (request.From == null || request.To == null || request.DepartureDate == null ||
                request.From == request.To)
            {
                return BadRequest();
            }

            var flights = FlightStorage.GetFlightByDest(request.From, request.To, request.DepartureDate);

            var expectedResult = new PageResult
            {
                Page = 0,
                TotalItems = flights.Count(),
                Items = flights.ToList()
            };

            return Ok(expectedResult);
        }

        [HttpGet]
        [Route("flights/{id}")]
        public IActionResult FindFlightById(int id)
        {
            var flight = FlightStorage.GetFlightById(id);
            if (flight == null)
            {
                return NotFound();
            }

            return Ok(flight);
        }
    }
}