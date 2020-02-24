using Demo.RentalRepairs.Domain.Enums;

namespace Demo.RentalRepairs.Domain.ValueObjects
{
    public class LoggedUser
    {
        public LoggedUser(string emailAddress, UserRolesEnum userRole = UserRolesEnum.Anonymous ,  string propCode = "", string unitNumber = "")
        {
            Login = emailAddress;
            UserRole = userRole;
            PropCode = propCode;
            UnitNumber = unitNumber;
        }
        public string Login { get; set; }
        public string PropCode { get; private set; }
        public string UnitNumber { get; private set; }
        public  UserRolesEnum UserRole { get; private set;  }
    }
}
