using FluentValidation;
using Reda.Dtos;

namespace Reda.Validators
{
    public class ValidationAddAddress : AbstractValidator<AddAddressDto>
    {
        public ValidationAddAddress()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.City)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Details)
                .NotEmpty()
                .MaximumLength(250);

            RuleFor(x => x.Phone)
                .NotEmpty()
                .Matches(@"^\+?[0-9]{10,15}$")
                .WithMessage("Phone must contain 10 to 15 digits.");
        }
    }
}
