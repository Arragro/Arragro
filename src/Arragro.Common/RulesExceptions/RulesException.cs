using System;
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
        }

        protected void Add(RulesException errors)
        {
            foreach (var error in errors.Errors)
            {
                Errors.Add(error);
            }
        }

        private IEnumerable<string> ThisErrors()
        {
            var output = new List<string>();

            var thisErrors = Errors.Where(x => x.Property == ThisObject);

            if (thisErrors.Any())
            {
                foreach (var thisError in thisErrors)
                {
                    if (string.IsNullOrEmpty(thisError.Prefix))
                        output.Add(string.Format("{0}", thisError.Message));
                    else
                        output.Add(string.Format("{0} - {1}", thisError.Prefix, thisError.Message));
                }
            }

            return output;
        }

        public IEnumerable<string> GetErrorMessages()
        {
            return ThisErrors();
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
            var errors = ThisErrors();

            output.AppendLine("The following error is against this object:\n");
            foreach (var error in errors)
                output.AppendLine($"\t{error}");

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

        private IEnumerable<string> ThisErrors()
        {
            var output = new List<string>();

            var propertyErrors = Errors.Where(x => x.Property != ThisObject);

            foreach (var propertyError in propertyErrors)
            {
                if (string.IsNullOrEmpty(propertyError.Prefix))
                    output.Add(string.Format("{0} - {1}", propertyError.GetPropertyPath(), propertyError.Message));
                else
                    output.Add(string.Format("{0} - {1} - {2}", propertyError.Prefix, propertyError.GetPropertyPath(), propertyError.Message));
            }

            return output;
        }

        public new IEnumerable<string> GetErrorMessages()
        {
            var baseErrors = base.GetErrorMessages().ToList();
            baseErrors.AddRange(this.ThisErrors());
            return baseErrors;
        }

        public override string ToString()
        {
            var output = new StringBuilder();

            var thisErrors = base.ToString();
            var errors = ThisErrors();

            if (errors.Any())
            {
                if (!string.IsNullOrEmpty(thisErrors))
                    output.AppendLine("\n\nWith errors against the following properties:\n");
                else
                    output.AppendLine("The following property errors are against this object:\n");

                foreach (var error in errors)
                    output.AppendLine($"\t{error}");
            }

            return thisErrors + output.ToString();
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
