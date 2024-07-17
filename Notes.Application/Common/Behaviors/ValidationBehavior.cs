using FluentValidation;
using FluentValidation.Results;
using MediatR;
using ValidationException = Notes.Application.Common.Exceptions.ValidationException;

namespace Notes.Application.Common.Behaviors
{
	public class ValidationBehavior<TRequest, TResponse> :
		IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
	{
		IEnumerable<IValidator<TRequest>> _validators;

		public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
		{
			_validators = validators;
		}

		public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
		{
			var failures = _validators
				.Select(v => v.Validate(request))
				.SelectMany(result => result.Errors)
				.Where(failure => failure != null)
				.ToList();

			if (failures.Count != 0)
			{
				string excMessage = CreateExceptionMessage(failures);
				throw new ValidationException(excMessage);

			}

			return await next();
		}

		string CreateExceptionMessage(List<ValidationFailure> failures) =>
			failures
				.Select(f => f.ErrorMessage)
				.Aggregate((lastMessage, currentMessage) => $"{lastMessage}; {currentMessage}");
	}
}
