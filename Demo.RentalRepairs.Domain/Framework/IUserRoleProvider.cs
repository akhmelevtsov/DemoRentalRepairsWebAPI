using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.RentalRepairs.Domain.Framework
{
    public interface IUserRoleProvider<TUserRoleEnum>
    {
        TUserRoleEnum UserRole { get; set;  }
    }
}
