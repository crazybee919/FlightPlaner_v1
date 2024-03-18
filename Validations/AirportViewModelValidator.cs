using FlightPlanner.Models;
using FluentValidation;

namespace FlightPlanner.Validations;

public class AirportViewModelValidator: AbstractValidator<AirportViewModel>
{
    public AirportViewModelValidator()
    {
        RuleFor(request => request.Airport).NotEmpty();
        RuleFor(request => request.City).NotEmpty();
        RuleFor(request => request.Country).NotEmpty();
    }
}