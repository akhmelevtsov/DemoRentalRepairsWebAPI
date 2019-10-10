using Demo.RentalRepairs.Domain.Framework;

namespace Demo.RentalRepairs.Domain.ValueObjects
{
    public class PropertyAddress : Value 
    {
        public string StreetNumber { get; set;  }
        public string StreetName { get; set;  }
        public string City { get; set;  }
        public string PostalCode { get; set;  }
    }
}
