using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Arragro.Common.BusinessRules
{
    /*
     * Taken from Steve Sanderson's Pro ASP.Net MVC 2 book around validation
     * of models, doesn't seem to be in the later books.
     * It is useful for the validation to occur at the service layer (business
     * layer) as the service layer then doesn't depend on MVC at all.  It will
     * throw the RulesException IF there are any error.  When it does, there
     * is an extension in another Arragro library that will copy these issues
     * to the ModelState.  ModelState is still validated by the MVC framework.
     */

    [Serializable]
    public class RuleViolation
    {
        public string Prefix { get; set; }

        public LambdaExpression Property { get; set; }

        public string Message { get; set; }
    }

    [Serializable]
    public class RulesException : Exception
    {
        public readonly IList<RuleViolation> Errors = new List<RuleViolation>();
        private readonly Expression<Func<object, object>> ThisObject = x => x;

        public void ErrorForModel(string message)
        {
            Errors.Add(new RuleViolation { Property = ThisObject, Message = message });
        }

        protected void Add(RulesException errors)
        {
            foreach (var error in errors.Errors)
            {
                Errors.Add(error);
            }
        }

        protected new virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Errors", Errors);
        }
    }

    [Serializable]
    public class RulesException<TModel> : RulesException
    {
        public void ErrorFor<TProperty>(Expression<Func<TModel, TProperty>> property,
                                        string message, string prefix = "")
        {
            Errors.Add(new RuleViolation { Property = property, Message = message, Prefix = prefix });
        }

        public void ErrorsForValidationResults(IEnumerable<ValidationResult> validationResults)
        {
            var type = typeof(TModel);
            foreach (var validationResult in validationResults)
            {
                foreach (var memberName in validationResult.MemberNames)
                {
                    var parameterExpression = Expression.Parameter(type);
                    var memberExpression = Expression.Property(parameterExpression, memberName);
                    var conversion = Expression.Convert(memberExpression, typeof(object));
                    var property = Expression.Lambda<Func<TModel, Object>>(conversion, parameterExpression);

                    Errors.Add(
                        new RuleViolation
                        {
                            Property = property,
                            Message = validationResult.ErrorMessage
                        });
                }
            }
        }
    }

    public static class RulesExceptionExtensions
    {
        public static bool ContainsErrorForProperty(this RulesException ex, string propertyName)
        {
            return ex.Errors.Any(x => x.Property.Body.ToString().Contains(string.Format(".{0}", propertyName)));
        }
    }
}