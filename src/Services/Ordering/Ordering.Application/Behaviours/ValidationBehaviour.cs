using FluentValidation;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
//make sure that we are using our own ValidationException class and not FluentValidations class
using ValidationException = Ordering.Application.Exceptions.ValidationException;

namespace Ordering.Application.Behaviours
{
    //this is our preprocessor class, every single mediator request has to reach this class
    //In this class, we collect all the validations and run the validate methods before executing the request (good feature for performing validation middleware, by intercepting mediator requests) 
    //implements mediator interface IPipelineBehaviour, we intercept before or after the handler class
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        //we collect all IValidator objects from the assembly (CheckoutOrderCommandValidator and UpdateOrderCommandValidator)
        //we can do this because the validator classes extend AbstractValidator, which implements IValidator
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        //next parameter: after we performed the validation behaviour, we can call the 'next' method in order to run an other behaviour
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if(_validators.Any())
            {
                //you can perform some of the validations using FluentValidation context
                var context = new ValidationContext<TRequest>(request);

                //completes when all the tasks have been completed
                //select all validators one by one and perform the ValidateAsync method
                var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));

                //check if any errors are not null
                //the where clause refers to r.Errors, which is a list of ValidationFailure
                //validationResults.Select(r => r.Errors) would return a list of list, selectmany flattens this

                #region commentSelectMany
                /* example:
                 * 
                 * public class PhoneNumber
                {
                    public string Number { get; set; }
                }

                public class Person
                {
                    public IEnumerable<PhoneNumber> PhoneNumbers { get; set; }
                    public string Name { get; set; }
                }

                IEnumerable<Person> people = new List<Person>();

                // Select gets a list of lists of phone numbers
                IEnumerable<IEnumerable<PhoneNumber>> phoneLists = people.Select(p => p.PhoneNumbers);

                // SelectMany flattens it to just a list of phone numbers.
                IEnumerable<PhoneNumber> phoneNumbers = people.SelectMany(p => p.PhoneNumbers);
                 */
                #endregion

                var validationFailures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

                if(validationFailures.Count != 0)
                {
                    throw new ValidationException(validationFailures);
                }
            }

            //after you handle your pipeline operations, you have to call the 'next' method in order to continue our requests, pipeline our requesthandler method in the mediator
            return await next();
        }
    }
}
