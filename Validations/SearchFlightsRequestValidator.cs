using FlightPlanner.Core.Models;
using FluentValidation;

namespace FlightPlanner.Validations;

public class SearchFlightsRequestValidator : AbstractValidator<SearchFlightsRequest>
{
    public SearchFlightsRequestValidator()
    {
        RuleFor(request => request.From).NotNull();
        RuleFor(request => request.To).NotNull();
        RuleFor(request => request.DepartureDate).NotNull();
    }
}