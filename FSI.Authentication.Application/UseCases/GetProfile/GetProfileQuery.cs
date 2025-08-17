using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.UseCases.GetProfile
{
    public sealed record GetProfileQuery(string Email);
}
