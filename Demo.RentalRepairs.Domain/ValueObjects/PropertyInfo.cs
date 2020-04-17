using System;
using System.Collections.Generic;
using System.Text;
using Demo.RentalRepairs.Domain.Framework;

namespace Demo.RentalRepairs.Domain.ValueObjects
{
    public class PropertyInfo : Value
    {
        public PropertyInfo(string name, string code, PropertyAddress address, string phoneNumber, PersonContactInfo superintendent, List<string> units)
        {
            Name = name;
            Code = code;
            Address = address;
            PhoneNumber = phoneNumber;
            Superintendent = superintendent;
            Units = units;
        }
        public string Name { get;  set; }
        public string Code { get;  set; }
        public PropertyAddress Address { get;  set; }
        public string PhoneNumber { get;  set; }
        public PersonContactInfo Superintendent { get;  set; }
        public List<string> Units { get;  set; }

    }
}
