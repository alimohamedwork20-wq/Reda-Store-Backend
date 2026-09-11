using FluentValidation;
using Reda.Dtos;

namespace Reda.Validators
{
    public class ValidationChangeContact : AbstractValidator<ChangeContactDto>
    {
        public ValidationChangeContact()
        {
            RuleFor(x => x.IdContact)
                .GreaterThan(0);

            RuleFor(x => x.messageReply)
                .NotEmpty()
                .MaximumLength(1000)
                .When(x => x.messageReply != null);
        }
    }
}
