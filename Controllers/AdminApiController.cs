using AutoMapper;
using FlightPlanner.Core.Models;
using FlightPlanner.Models;
using FlightPlannerCore.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace FlightPlanner.Controllers
{
    [Authorize]
    [Route("admin-api")]
    [ApiController]
    public class AdminApiController : ControllerBase
    {
        private readonly IFlightService _flightsService;
        private readonly IMapper _mapper;
        private readonly IValidator<AddFlightRequest> _validator;

        public AdminApiController(IFlightService flightsService, IMapper mapper, IValidator<AddFlightRequest> validator)
        {
            _flightsService = flightsService;
            _mapper = mapper;
            _validator = validator;
        }

        private static readonly object lockObject = new object();

        [HttpDelete]
        [Route("flights/{id}")]
        public IActionResult DeleteFlight(int id)
        {
            lock (lockObject)
            {
                var flight = _flightsService.GetFullFlightById(id);
                if (flight == null)
                {
                    return Ok();
                }

                _flightsService.Delete(flight);

                return Ok();
            }
        }

        [HttpGet]
        [Route("flights/{id}")]
        public IActionResult GetFlight(int id)
        {
            lock (lockObject)
            {
                var flight = _flightsService.GetFullFlightById(id);
                if (flight == null)
                {
                    return NotFound();
                }

                return Ok(_mapper.Map<AddFlightResponse>(flight));
            }
        }

        [HttpPut]
        [Route("flights")]
        public IActionResult AddFlight(AddFlightRequest request)
        {
            lock (lockObject)
            {
                if (request.From.Airport.Trim().ToLower() == request.To.Airport.Trim().ToLower())
                {
                    return BadRequest();
                }
                //if time not wierd
                DateTime departureTime, arrivalTime;
                if (DateTime.TryParse(request.DepartureTime, out departureTime) &&
                    DateTime.TryParse(request.ArrivalTime, out arrivalTime))
                {
                    if (departureTime >= arrivalTime)
                    {
                        return BadRequest();
                    }
                }
                
                var allFlights = _flightsService.GetAll();

                if (allFlights.Any(exsistingFlight => exsistingFlight.To.AirportCode == request.To.Airport 
                                                      && exsistingFlight.From.AirportCode == request.From.Airport 
                                                      && exsistingFlight.Carrier == request.Carrier
                                                      && exsistingFlight.ArrivalTime == request.ArrivalTime
                                                      && exsistingFlight.DepartureTime == request.DepartureTime))
                {
                    return Conflict();
                }

                var validationResult = _validator.Validate(request);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }


                var flight = _mapper.Map<Flight>(request);
                _flightsService.Create(flight);

                return Created("", _mapper.Map<AddFlightResponse>(flight));
            }
        }
    }
}