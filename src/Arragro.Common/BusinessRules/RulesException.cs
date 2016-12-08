using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

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
     
    public class RuleViolation
    {
        public string Prefix { get; set; }

        public LambdaExpression Property { get; set; }

        public string Message { get; set; }

        public string GetPropertyPath()
        {
            var stack = new Stack<string>();

            MemberExpression me;
            switch (Property.Body.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    var ue = Property.Body as UnaryExpression;
                    me = ((ue != null) ? ue.Operand : null) as MemberExpression;
                    break;
                default:
                    me = Property.Body as MemberExpression;
                    break;
            }

            while (me != null)
            {
                stack.Push(me.Member.Name);
                me = me.Expression as MemberExpression;
            }

            return string.Join(".", stack.ToArray());
        }
    }
    
    public class RulesException : Exception
    {
        public readonly IList<RuleViolation> Errors = new List<RuleViolation>();
        protected readonly Expression<Func<object, object>> ThisObject = x => x;

        protected RulesException() { }

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

        protected string ThisErrors()
        {
            var output = new StringBuilder();

            var thisErrors = Errors.Where(x => x.Property == ThisObject);

            if (thisErrors.Any())
            {
                output.AppendLine("The following error is against this object:\n");
                foreach (var thisError in thisErrors)
                {
                    if (string.IsNullOrEmpty(thisError.Prefix))
                        output.AppendLine(string.Format("\t{0}", thisError.Message));
                    else
                        output.AppendLine(string.Format("\t{0} - {1}", thisError.Prefix, thisError.Message));
                }
            }

            return output.ToString();
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
                    output.AppendLine(ThisErrors());
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
                output.AppendLine(ThisErrors());

                throw new RulesException(output.ToString(), this);
            }
        }

        public override string ToString()
        {
            var output = new StringBuilder();

            return ThisErrors();
        }
    }
    
    public class RulesException<TModel> : RulesException
    {
        public RulesException() : base() { }

        private RulesException(string message, RulesException rulesException) : base(message, rulesException) { }

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

        public override string ToString()
        {
            var output = new StringBuilder();

            var thisErrors = ThisErrors();
            var propertyErrors = Errors.Where(x => x.Property != ThisObject);

            if (propertyErrors.Any())
            {
                if (!string.IsNullOrEmpty(thisErrors))
                    output.AppendLine("\n\nWith errors against the following properties:\n");
                else
                    output.AppendLine("The following property errors are against this object:\n");
                foreach (var propertyError in propertyErrors)
                {
                    if (string.IsNullOrEmpty(propertyError.Prefix))
                        output.AppendLine(string.Format("\t{0} - {1}", propertyError.GetPropertyPath(), propertyError.Message));
                    else
                        output.AppendLine(string.Format("\t{0} - {1} - {2}", propertyError.Prefix, propertyError.GetPropertyPath(), propertyError.Message));
                }
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