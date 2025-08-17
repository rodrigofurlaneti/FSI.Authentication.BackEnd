using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.Validators
{
    public interface IValidator<in T> { void ValidateAndThrow(T instance); }
}
