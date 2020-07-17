using System;
using System.Collections.Generic;
using System.Text;
using Demo.RentalRepairs.Domain.ValueObjects;

namespace Demo.RentalRepairs.Infrastructure.Repositories.EF.Entities
{
    public class WorkerTbl
    {
        public Guid ID { get; set; }
        public PersonContactInfo ContactInfo { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
