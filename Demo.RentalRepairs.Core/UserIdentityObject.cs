using Demo.RentalRepairs.Domain.Enums;

namespace Demo.RentalRepairs.Core
{
    public class UserIdentityObject
    {
        public UserRolesEnum UserRole { get; set;  }
        public string Email { get; set;  }
    }
}
