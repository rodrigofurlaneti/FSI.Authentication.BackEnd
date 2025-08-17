using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.Notifications
{
    public sealed record UserRegisteredNotification(
        Guid UserId, string Email, string Name, string Profile, DateTime When);
}
