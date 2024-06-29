using FluentValidation;
using MediatR;
using ValidationException = Notes.Application.Common.Exceptions.ValidationException;

namespace Notes.Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> :
        IPipelineBehavior<TRequest, TRequest> where TRequest : IRequest<TResponse>
    {
        IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) 
        {
            _validators = validators;
        }

        public async Task<TRequest> Handle(TRequest request, RequestHandlerDelegate<TRequest> next, CancellationToken cancellationToken)
        {
            var failures = _validators
                .Select(v => v.Validate(request))
                .SelectMany(result => result.Errors)
                .Where(failure => failure != null)
                .ToList();

            string errorMessages = failures
                .Select(f => f.ErrorMessage)
                .Aggregate((lastMessage, currentMessage) => $"{lastMessage}; {currentMessage}");

            if (failures.Count != 0)
                throw new ValidationException(errorMessages);

            return await next();
        }
    }
}
