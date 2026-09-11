using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Reda.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument == null)
                    continue;

                var validatorType = typeof(IValidator<>)
                    .MakeGenericType(argument.GetType());

                var validator = _serviceProvider.GetService(validatorType);

                if (validator is not IValidator actualValidator)
                    continue;
                var validationResult = await actualValidator.ValidateAsync(
                    new ValidationContext(argument));

                if (!validationResult.IsValid)
                {
                    throw new ValidationException(validationResult.Errors);
                }
            }

            await next();
        }
    }
}