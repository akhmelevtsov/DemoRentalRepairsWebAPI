using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.RentalRepairs.Core.Security
{
    public enum PolicyEnum
    {
        AnyLoggedUser,
        RegisteredTenant,
        RegisteredSuperintendent,
        RegisteredAdministrator,
        RegisteredWorker
    }
}
