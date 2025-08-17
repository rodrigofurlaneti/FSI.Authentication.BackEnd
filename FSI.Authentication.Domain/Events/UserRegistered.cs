using FSI.Authentication.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Domain.Events
{
    public sealed record UserRegistered(Guid UserId, string Email, string Name, string Profile, DateTime When) : IDomainEvent;

}
