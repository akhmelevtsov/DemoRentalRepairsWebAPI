using System;
using Demo.RentalRepairs.Domain.Framework;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.Domain.ValueObjects;

namespace Demo.RentalRepairs.Domain.Entities
{
    public class Worker :Entity
    {
        readonly DomainValidationService _validationService = new DomainValidationService();
        public Worker( PersonContactInfo contactInfo,  DateTime? dateCreated = null, Guid? id = null) : base(dateCreated, id)
        {
            _validationService.Validate(contactInfo);
            PersonContactInfo = contactInfo;
          
        }

        public PersonContactInfo PersonContactInfo { get; set;  }
      
    }
}
