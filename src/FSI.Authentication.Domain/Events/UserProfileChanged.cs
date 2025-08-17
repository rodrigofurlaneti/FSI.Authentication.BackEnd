using FSI.Authentication.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Domain.Events
{
    public sealed record UserProfileChanged(Guid UserId, string OldProfile, string NewProfile, DateTime When) : IDomainEvent;

}
