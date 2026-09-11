using FluentValidation;
using Reda.Dtos;

namespace Reda.Validators
{
    public class ValidationAddContact : AbstractValidator<AddContactDto>
    {
        public ValidationAddContact()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MinimumLength(2)
                .MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Message)
                .NotEmpty()
                .MaximumLength(1000);
        }
    }
}
