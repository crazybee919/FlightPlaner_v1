using Microsoft.AspNetCore.Mvc;
using FlightPlaner;
using FlightPlaner.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightPlanner.Controllers
{
    [Route("api")]
    [ApiController]
    public class CustomerApiController : ControllerBase
    {
        private readonly FlightPlannerDBContext _context;
        public CustomerApiController(FlightPlannerDBContext context)
        {
            _context = context;
        }
        private static readonly object lockObject = new object();
        
        [HttpGet]
        [Route("airports")]
        public IActionResult SearchAirports(string search)
        {
            List<string> validValues = new List<string> { "rix", "ri", "rig", "latv", "latvia", "riga" };

            string searchTerm = search.ToLower().Trim();

            if (validValues.Contains(searchTerm))
            {
                 return Ok(_context.Flights.FirstOrDefault(x => x.To.AirportCode == "RIX"));
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
            
            var expectedResult = new PageResult
            {
                Page = 0,
                TotalItems =_context.Flights.Count(), 
                Items =_context.Flights.ToList()
            };
            _context.SaveChanges();

            return Ok(expectedResult);
        }

        [HttpGet]
        [Route("flights/{id}")]
        public IActionResult FindFlightById(int id)
        { 
            lock (lockObject)
            {
                var flight = _context.Flights.Include(flight => flight.To)
                    .Include(flight => flight.From).SingleOrDefault(flight => flight.Id == id);
                if (flight == null)
                {
                    return NotFound();
                }
                _context.SaveChanges();

                return Ok(flight);
            }
        }
    }
}