using FluentValidation;
using MediatR;
using Ordering.Application.Abstraction.Messaging;
using Ordering.Domain.Sahred;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Behaviors
{
    public class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
                                                          where TRequest : IRequest<TResponse>
                                                          where TResponse : Result
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        public ValidatorBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())//if no have any validators for command 
            {
                return await next();
            }
            var context = new ValidationContext<TRequest>(request);
            var validationFailures = await Task.WhenAll(
            _validators.Select(validator => validator.ValidateAsync(context)));

            Error[] errors = validationFailures
            .Where(validationResult => !validationResult.IsValid)
            .SelectMany(validationResult => validationResult.Errors)
            .Select(validationFailure => new Error(
                validationFailure.PropertyName,
                validationFailure.ErrorMessage)).Distinct().ToArray();

            if (errors.Any())
            {
                //  throw new Exceptions.ValidationException(errors);
                //  you can handle iy using execpition middlware handle and  context.Response.WriteAsJsonAsync(problemDetails);
                //or return direct unvalidation result in endPoint response like this 

               return CreateValidationResult<TResponse>(errors);
            }

            var response = await next();

            return response;

        }
        public static TResult CreateValidationResult<TResult>(Error[] errors) 
            where TResult : Result
        {
            if (typeof(TResult) == typeof(Result))
            {
                return (ValidationResult<TResult>.WithErrors(errors) as TResult)!;
            }
            object? validationResult = typeof(ValidationResult<>)
                                     .GetGenericTypeDefinition()
                                     .MakeGenericType(typeof(TResult).GenericTypeArguments[0])
                                     .GetMethod(nameof(ValidationResult<TResult>.WithErrors))!
                                     .Invoke(null, new object?[] { errors})!;

            return (TResult)validationResult;
        }
    }
}
