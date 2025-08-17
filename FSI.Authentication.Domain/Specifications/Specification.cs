using System;
using System.Linq.Expressions;

namespace FSI.Authentication.Domain.Specifications
{
    /// <summary>
    /// Base mínima para specifications com um critério (Expression).
    /// </summary>
    public abstract class Specification<T>
    {
        protected Specification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria ?? (_ => true);
        }

        public Expression<Func<T, bool>> Criteria { get; }
    }
}
