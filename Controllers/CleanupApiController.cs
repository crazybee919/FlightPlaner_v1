using Microsoft.AspNetCore.Mvc;

namespace FlightPlaner.Controllers
{
    [Route("testing-api")]
    [ApiController]
    public class CleanupApiController : ControllerBase
    {
        private readonly FlightPlannerDBContext _context;

        public CleanupApiController(FlightPlannerDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("clear")]
        public IActionResult Clear()
        {
            _context.Flights.RemoveRange(_context.Flights);
            _context.Airports.RemoveRange(_context.Airports);
            _context.SaveChanges();
            return Ok();
        }
    }
}