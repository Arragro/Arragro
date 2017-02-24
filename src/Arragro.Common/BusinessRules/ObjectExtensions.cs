using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Arragro.Common.BusinessRules
{
    public static class ObjectExtensions
    {
        public static IList<ValidationResult> ValidateModelProperties<TModel>(this TModel model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }
    }
}
