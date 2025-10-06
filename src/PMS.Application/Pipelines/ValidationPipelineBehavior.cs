using FluentValidation;
using MediatR;
using PMS.Application.Wrappers;

namespace PMS.Application.Pipelines;

public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IValidateSelf
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators?.Any() ?? false)
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(_validators.Select(x => x.ValidateAsync(context, cancellationToken)));

            if (!validationResults.Any(x => x.IsValid))
            {
                var errors = validationResults.SelectMany(x => x.Errors)
                    .Where(x => x != null)
                    .Select(x => x.ErrorMessage)
                    .ToList();

                return (TResponse)await ResponseWrapper.FailAsync(errors);
            }
        }

        return await next(cancellationToken);
    }
}
