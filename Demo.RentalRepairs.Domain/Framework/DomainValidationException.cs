using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Results;

namespace Demo.RentalRepairs.Domain.Framework
{
    public class DomainValidationException : ValidationException
    {
        public string ErrorCode { get; }

       
        public DomainValidationException(string errorCode,  IEnumerable<ValidationFailure> errors) : base(errors)
        {
            ErrorCode = errorCode;
        }
    }
}
