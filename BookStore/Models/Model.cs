using BookStore.Exceptions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BookStore.Models
{
    /// <summary>
    /// Abstract class that implements usefull model methods and serve as a base for all aplication models.
    /// </summary>
    public abstract class Model
    {
        /// <summary>
        /// <para>Validates a Model acording to it's data annotations.</para>
        /// <para>Does the same as Model.IsValid but allows for the model to be validates outside of the cotroller context.</para>
        /// <para>Throws BadRequestException if any validation error is found.</para>
        /// </summary>
        /// <exception cref="BadRequestException" />
        public virtual void Validate()
        {
            ValidationContext validationContext = new ValidationContext(this);
            List<ValidationResult> results = new List<ValidationResult>();
            Validator.TryValidateObject(this, validationContext, results, true);

            if (results.Any())
            {
                throw new BadRequestException(results);
            }
        }
    }
}
