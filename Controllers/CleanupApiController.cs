using FlightPlanner.Extensions;
using FlightPlanner.UseCases.CleanUp;
using FlightPlannerCore.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Controllers
{
    [Route("testing-api")]
    [ApiController]
    public class CleanupApiController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CleanupApiController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("clear")]
        public async Task<IActionResult> Clear()
        {
            return (await _mediator.Send(new DataCleanupCommand())).ToActionResult();
        }
    }
}