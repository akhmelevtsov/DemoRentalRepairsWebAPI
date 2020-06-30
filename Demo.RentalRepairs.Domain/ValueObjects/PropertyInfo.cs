using System.Collections.Generic;
using Demo.RentalRepairs.Domain.Framework;

namespace Demo.RentalRepairs.Domain.ValueObjects
{
    public class PropertyInfo : Value
    {
        public PropertyInfo(string name, string code, PropertyAddress address, string phoneNumber, PersonContactInfo superintendent, List<string> units, string noReplyEmailAddress)
        {
            Name = name;
            Code = code;
            Address = address;
            PhoneNumber = phoneNumber;
            Superintendent = superintendent;
            Units = units;
            NoReplyEmailAddress = noReplyEmailAddress;
        }
        public string Name { get;  }
        public string Code { get; }
        public PropertyAddress Address { get;   }
        public string PhoneNumber { get;   }
        public PersonContactInfo Superintendent { get;   }
        public List<string> Units { get; }
        public string NoReplyEmailAddress { get; }
    }
}
