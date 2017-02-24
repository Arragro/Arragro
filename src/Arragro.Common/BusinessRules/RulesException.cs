using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Arragro.Common.BusinessRules
{
    public class RulesExceptionDto
    {
        public string ErrorMessage { get; protected set; }
        public IDictionary<string, object> Errors { get; protected set; }
        public List<string> ErrorMessages { get; protected set; }

        public RulesExceptionDto()
        {
            Errors = new Dictionary<string, object>();
            ErrorMessages = new List<string>();
        }

        protected void processDictionaries(IEnumerable<RulesException> rulesExceptions)
        {
            foreach (var rulesException in rulesExceptions)
            {
                var errors = rulesException.GetErrorDictionary().ToList();
                foreach (var error in errors)
                {
                    object value;
                    if (Errors.TryGetValue(error.Key, out value))
                    {
                        Errors.Add($"{rulesException.TypeName}.{error.Key}", error.Value);
                    }
                    else
                        Errors.Add(error.Key, error.Value);
                }

            }
        }

        public RulesExceptionDto(RulesException rulesException) : this()
        {
            ErrorMessage = rulesException.ToString();
            Errors = rulesException.GetErrorDictionary();
            ErrorMessages.AddRange(rulesException.GetErrorMessages());
        }

        public RulesExceptionDto(IEnumerable<RulesException> rulesExceptions) : this()
        {
            processDictionaries(rulesExceptions);
            rulesExceptions.SelectMany(x => x.GetErrorMessages()).ToList().ForEach(x => ErrorMessages.Add(x));
        }
    }

    public class RulesExceptionDto<TModel> : RulesExceptionDto
    {
        public RulesExceptionDto() : base() { }

        public RulesExceptionDto(RulesException<TModel> rulesException) : this()
        {
            ErrorMessage = rulesException.ToString();
            Errors = rulesException.GetErrorDictionary();
            ErrorMessages.AddRange(rulesException.GetErrorMessages());
        }

        public RulesExceptionDto(IEnumerable<RulesException<TModel>> rulesExceptions) : this()
        {
            foreach (var rulesException in rulesExceptions)
                ErrorMessage += rulesException.ToString();
            processDictionaries(rulesExceptions);
            rulesExceptions.SelectMany(x => x.GetErrorMessages()).ToList().ForEach(x => ErrorMessages.Add(x));
        }
    }

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

        public KeyValuePair<string, object> KeyValuePair
        {
            get
            {
                if (string.IsNullOrEmpty(Prefix))
                    return new KeyValuePair<string, object>(GetPropertyPath(), Message);
                else
                    return new KeyValuePair<string, object>(string.Format("{0}.{1}", Prefix, GetPropertyPath()), Message);
            }
        }
    }
    
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