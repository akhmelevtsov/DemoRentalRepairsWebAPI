using System;
using Demo.RentalRepairs.Domain.Framework;
using Demo.RentalRepairs.Domain.ValueObjects;

namespace Demo.RentalRepairs.Domain.Entities
{
    public class Worker :Entity
    {
        public Worker( PersonContactInfo contactInfo,  DateTime? dateCreated = null, Guid? id = null) : base(dateCreated, id)
        {
          
            PersonContactInfo = contactInfo;
          
        }

        public PersonContactInfo PersonContactInfo { get; set;  }
      
    }
}
