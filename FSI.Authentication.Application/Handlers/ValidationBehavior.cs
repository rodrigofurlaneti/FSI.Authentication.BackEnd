using FSI.Authentication.Application.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.Handlers
{
    public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IValidator<TRequest>? validator;
        public ValidationBehavior(IValidator<TRequest>? validator = null) => this.validator = validator;

        public Task<TResponse> HandleAsync(TRequest request, CancellationToken ct, Func<Task<TResponse>> next)
        {
            validator?.ValidateAndThrow(request);
            return next();
        }
    }
}
