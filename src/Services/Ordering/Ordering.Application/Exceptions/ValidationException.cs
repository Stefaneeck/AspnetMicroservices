using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ordering.Application.Exceptions
{
    public class ValidationException : ApplicationException
    {
        //chain to base class constructor
        public ValidationException()
           : base("One or more validation errors have occured.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        //chain to constructor in this class
        //if any error occurs in one of the validator classes RuleFor, it's a validationfailure
        public ValidationException(IEnumerable<ValidationFailure> failures)
           : this()
        {
            Errors = failures
                //this will write the property name and error message of the validationfailure (comes from the RuleFor method in the validator class)
                .GroupBy(f => f.PropertyName, f => f.ErrorMessage)
                //these are 2 different parameters, both lambdas. first lamdba returns string, the second returns string[]
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
        }

        public IDictionary<string, string[]> Errors { get; set; }
    }
}
