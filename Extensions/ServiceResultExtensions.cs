using System.Net;
using FlightPlanner.UseCases.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Extensions;

public static class ServiceResultExtensions
{
    public static IActionResult ToActionResult(this ServiceResult result)
    {
        switch (result.Status)
        {
            case HttpStatusCode.NotFound :
                return new NotFoundResult();
            case HttpStatusCode.Created :
                return new CreatedResult(string.Empty, result.ResultObject);
            case HttpStatusCode.OK:
                return new OkObjectResult(result.ResultObject);
            case HttpStatusCode.BadRequest:
                return new BadRequestObjectResult(result.ResultObject);
            case HttpStatusCode.Conflict:
                return new ConflictObjectResult(result.ResultObject);
            default:
                return new OkResult();
        }
    }
}