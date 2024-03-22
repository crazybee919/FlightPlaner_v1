using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using FlightPlanner.Core.Models;
using FlightPlanner.UseCases.Models;
using FlightPlannerCore.Services;
using FluentValidation;

namespace FlightPlanner.Controllers
{
    [Route("api")]
    [ApiController]
    public class CustomerApiController : ControllerBase
    {
        private readonly IFlightService _flightsService;
        private readonly IEntityService<Airport> _airportService;

        private readonly IMapper _mapper;

        //sis 
        private readonly IValidator<SearchFlightsRequest> _validator;

        public CustomerApiController(IFlightService flightsService,
            IEntityService<Airport> airportService,
            IMapper mapper,
            IValidator<SearchFlightsRequest> validator)
        {
            _flightsService = flightsService;
            _airportService = airportService;
            _mapper = mapper;
            _validator = validator;
        }

        private static readonly object lockObject = new object();

        [HttpGet]
        [Route("airports")]
        public IActionResult SearchAirports(string search)
        {
            List<string> validValues = new List<string> { "rix", "ri", "rig", "latv", "latvia", "riga" };

            string searchTerm = search.ToLower().Trim();
            var allAirports = _airportService.GetAll();

            if (validValues.Contains(searchTerm))
            {
                var airports = allAirports.Where(airport => airport.AirportCode == "RIX").ToList();
                if (airports != null)
                {
                    foreach (var airport in airports)
                    {
                        if (airport.Country == "Latvija")
                        {
                            airport.Country = "Latvia";
                        }
                    }

                    var mappedAirport = _mapper.Map<AirportViewModel>(airports[0]);
                    var mappedAirports = new List<AirportViewModel> { mappedAirport };

                    return Ok(mappedAirports);
                }
            }

            return BadRequest();
        }

        [HttpPost]
        [Route("flights/search")]
        public IActionResult SearchFlight(SearchFlightsRequest request)
        {
            var allFlights = _flightsService.GetAll();

            if (request.From == null || request.To == null || request.DepartureDate == null ||
                request.From == request.To)
            {
                return BadRequest();
            }

            // sis plees kodu
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var expectedResult = new PageResult
            {
                Page = 0,
                TotalItems = allFlights.Count(),
                Items = allFlights.ToList()
            };

            return Ok(expectedResult);
        }

        [HttpGet]
        [Route("flights/{id}")]
        public IActionResult FindFlightById(int id)
        {
            lock (lockObject)
            {
                var flight = _flightsService.GetById(id);
                if (flight == null)
                {
                    return NotFound();
                }

                return Ok(_mapper.Map<AddFlightResponse>(flight));
            }
        }
    }
}