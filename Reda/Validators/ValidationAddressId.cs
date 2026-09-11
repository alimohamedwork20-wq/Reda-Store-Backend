using FluentValidation;
using Reda.Dtos;

namespace Reda.Validators
{
    public class ValidationAddressId : AbstractValidator<ChangeAndDeleteAddressDto>
    {
        public ValidationAddressId()
        {
            RuleFor(x => x.AddressId)
                .GreaterThan(0);
        }
    }
}
