using System;
using System.Collections.Generic;
using Demo.RentalRepairs.Domain.ValueObjects;

namespace Demo.RentalRepairs.Infrastructure.Repositories.EF.Entities
{
    public class TenantTbl
    {
        public Guid ID { get; set; }

        public PersonContactInfo ContactInfo { get; set; }
        public string  PropertyTblID { get; set; }  //Code
        public PropertyTbl Property { get; set; }
        public string UnitNumber { get;  set; }

        public  ICollection<TenantRequestTbl> Requests { get; set; }
        public DateTime DateCreated { get; set; }

    }
}
