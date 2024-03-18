using FlightPlannerCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Controllers
{
    [Route("testing-api")]
    [ApiController]
    public class CleanupApiController : ControllerBase
    {
        private readonly IFlightService _flightsService;

        public CleanupApiController(IFlightService flightsService)
        {
            _flightsService = flightsService;
        }

        [HttpPost]
        [Route("clear")]
        public IActionResult Clear()
        {
            var allFlights = _flightsService.GetAll();
            foreach (var flight in allFlights)
            {
                _flightsService.Delete(flight);
            }

            return Ok();
        }
    }
}