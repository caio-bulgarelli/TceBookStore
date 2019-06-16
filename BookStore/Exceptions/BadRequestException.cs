using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Exceptions
{
    public class BadRequestException : RequestException
    {
        public List<ValidationResult> ValidationResults { get; private set; }

        public BadRequestException() { }
        public BadRequestException(string message) : base(message) { }
        public BadRequestException(List<ValidationResult> validationResults)
        {
            ValidationResults = validationResults;
        }
    }
}
