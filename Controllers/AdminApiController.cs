using FlightPlanner.Extensions;
using FlightPlanner.UseCases.CleanUp;
using FlightPlanner.UseCases.Flights.AddFlight;
using FlightPlanner.UseCases.Flights.GetFlight;
using FlightPlanner.UseCases.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace FlightPlanner.Controllers
{
    [Authorize]
    [Route("admin-api")]
    [ApiController]
    public class AdminApiController : ControllerBase
    {
        private readonly IMediator _mediator;
        private static readonly object lockObject = new object();


        public AdminApiController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpDelete]
        [Route("flights/{id}")]
        public IActionResult DeleteFlight(int id)
        {
            lock (lockObject)
            {
                _mediator.Send(new DataCleanupCommand());
                return Ok();
            }
        }

        [HttpGet]
        [Route("flights/{id}")]
        public async Task<IActionResult> GetFlight(int id)
        {
            return (await _mediator.Send(new GetFlightQuery(id))).ToActionResult();
        }

        [HttpPut]
        [Route("flights")]
        public async Task<IActionResult> AddFlight(AddFlightRequest request)
        {
            lock (lockObject)
            {
                if (request.From.Airport.Trim().ToLower() == request.To.Airport.Trim().ToLower())
                {
                    return BadRequest();
                }

                DateTime departureTime, arrivalTime;
                if (DateTime.TryParse(request.DepartureTime, out departureTime) &&
                    DateTime.TryParse(request.ArrivalTime, out arrivalTime))
                {
                    if (departureTime >= arrivalTime)
                    {
                        return BadRequest();
                    }
                }
            }

            return (await _mediator.Send(new AddFlightCommand { AddFlightRequest = request })).ToActionResult();
        }
    }
}