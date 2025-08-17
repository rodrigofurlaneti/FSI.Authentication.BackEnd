using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.Notifications
{
    public sealed record FailedLoginNotification(Guid UserId, string Email, DateTime When);

}
