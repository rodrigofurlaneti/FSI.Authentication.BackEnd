using System;

namespace FSI.Authentication.Application.Exceptions
{
    public class ValidationAppException : Exception
    {
        public ValidationAppException(string message) : base(message) { }
    }

}
