using Demo.RentalRepairs.Domain.Framework;

namespace Demo.RentalRepairs.Domain.ValueObjects
{
    public class PersonContactInfo : Value
    {
        public string FirstName { get; set;  }
        public string LastName { get; set;  }
        public string EmailAddress { get; set;  }
        public string MobilePhone { get;  set; }

        public string GetFullName() => FirstName + " " + LastName;
    }
}
