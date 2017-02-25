﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Arragro.Common.RulesExceptions
{
    public class RulesException : Exception
    {
        public readonly IList<RuleViolation> Errors = new List<RuleViolation>();
        public readonly IList<string> ErrorMessages = new List<string>();
        protected readonly Expression<Func<object, object>> ThisObject = x => x;
        protected readonly Type Type;

        protected RulesException(Type type)
        {
            Type = type;
        }

        public string TypeName { get { return Type.Name; } }

        protected RulesException(string message, RulesException rulesException) : base(message)
        {
            Errors = rulesException.Errors;
        }

        public void ErrorForModel(string message)
        {
            Errors.Add(new RuleViolation { Property = ThisObject, Message = message });

            ErrorMessages.Add(message);
        }

        protected void Add(RulesException errors)
        {
            foreach (var error in errors.Errors)
            {
                Errors.Add(error);
                if (string.IsNullOrEmpty(error.Prefix))
                    ErrorMessages.Add($"{error.GetPropertyPath()} - {error.Message}");
                else
                    ErrorMessages.Add($"{error.Prefix} - {error.GetPropertyPath()} - {error.Message}");
            }
        }

        public override string Message
        {
            get
            {
                var thisErrors = Errors.Where(x => x.Property == ThisObject);
                var output = new StringBuilder(base.Message);

                if (thisErrors.Any())
                {
                    output.AppendLine("\n\n====================================");
                    output.AppendLine(ToString());
                }
                return output.ToString();
            }
        }

        public void ThrowException()
        {
            var thisErrors = Errors.Where(x => x.Property == ThisObject);
            var output = new StringBuilder(base.Message);

            if (thisErrors.Any())
            {
                output.AppendLine("\n\n====================================");
                output.AppendLine(ToString());

                throw new RulesException(output.ToString(), this);
            }
        }

        public override string ToString()
        {
            var output = new StringBuilder();

            output.AppendLine("The following error is against this object:\n");
            foreach (var error in Errors)
                output.AppendLine($"\t{error.Message}");

            return output.ToString();
        }

        public IDictionary<string, object> GetErrorDictionary()
        {
            return new Dictionary<string, object>(Errors.Select(x => x.KeyValuePair).ToDictionary(x => x.Key, x => x.Value));
        }
    }

    public class RulesException<TModel> : RulesException
    {
        public RulesException() : base(typeof(TModel)) { }

        private RulesException(string message, RulesException rulesException) : base(message, rulesException) { }

        public void ErrorFor<TProperty>(
            Expression<Func<TModel, TProperty>> property,
            string message, string prefix = "")
        {
            var propertyError = new RuleViolation { Property = property, Message = message, Prefix = prefix };
            Errors.Add(propertyError);

            if (string.IsNullOrEmpty(prefix))
                ErrorMessages.Add($"{propertyError.GetPropertyPath()} - {propertyError.Message}");
            else
                ErrorMessages.Add($"{propertyError.Prefix} - {propertyError.GetPropertyPath()} - {propertyError.Message}");
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

        public new void ThrowException()
        {
            var output = new StringBuilder(base.Message);

            if (Errors.Any())
            {
                output.AppendLine("\n\n====================================");
                output.AppendLine(ToString());
                throw new RulesException<TModel>(output.ToString(), this);
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