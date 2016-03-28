using Arragro.Common.BusinessRules;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Arragro.WebApi
{
    public static class RulesViolationExceptionExtensions
    {
        public static void CopyTo(this RulesException ex,
                                  ApiController controller)
        {
            CopyTo(ex, controller, controller.ModelState, null);
        }

        public static void CopyTo(this RulesException ex,
                                  ModelStateDictionary modelState)
        {
            CopyTo(ex, null, modelState, null);
        }

        static string GetPropertyName(LambdaExpression property)
        {
            var propertyInfo = (property.Body as MemberExpression).Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException("The lambda expression 'property' should point to a valid Property");
            }
            return propertyInfo.Name;
        }

        public static void CopyTo(this RulesException ex,
                                  ApiController controller,
                                  ModelStateDictionary modelState,
                                  string prefix)
        {
            prefix = string.IsNullOrEmpty(prefix) ? "" : prefix + ".";
            modelState.Clear();
            foreach (var propertyError in ex.Errors)
            {
                var errorPrefix = string.IsNullOrEmpty(propertyError.Prefix) ? prefix : propertyError.Prefix + ".";
                var key = propertyError.GetPropertyPath();
                modelState.AddModelError(errorPrefix + key, propertyError.Message);
            }
        }
    }
}
