using System;

namespace Arragro.Common.Exceptions
{
    public class ApiHttpException : Exception
    {
        public ApiHttpException(string message) : base(message) { }
    }
}
