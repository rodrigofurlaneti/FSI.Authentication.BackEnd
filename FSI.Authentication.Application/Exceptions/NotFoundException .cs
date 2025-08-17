using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.Exceptions
{
    public sealed class NotFoundException : AppException
    {
        public NotFoundException(string message) : base(message) { }
    }
}
