using System;

namespace Arragro.WebApi.Exceptions
{
    public class ApiHttpException : Exception
    {
        public ApiHttpException(string message) : base(message) { }
    }
}
