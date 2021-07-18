using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace GmodNET
{
    internal static class EnvironmentChecks
    {
        internal static bool IsDevelopmentEnvironemt()
        {
            if (Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") == Environments.Development)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
