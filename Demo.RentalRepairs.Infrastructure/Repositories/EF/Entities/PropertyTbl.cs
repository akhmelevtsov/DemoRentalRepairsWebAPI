using System;
using System.Collections.Generic;
using Demo.RentalRepairs.Domain.ValueObjects;

namespace Demo.RentalRepairs.Infrastructure.Repositories.EF.Entities
{
    public class PropertyTbl
    {
        public string ID { get; set; }  // Code

        public string Name { get;  set; }
       
       
        public PropertyAddress Address { get;  set; }
        public string PhoneNumber { get;  set; }
       
        public PersonContactInfo Superintendent { get;  set; }
        public string NoReplyEmailAddress { get; set; }
        public string LoginEmail { get; set; }
        public ICollection<TenantTbl> Tenants { get; set; }
     
        public string Units { get;  set; }  //List<string>

        public DateTime DateCreated { get; set;  }
        public Guid IdCreated { get; set;  }

    }
}
