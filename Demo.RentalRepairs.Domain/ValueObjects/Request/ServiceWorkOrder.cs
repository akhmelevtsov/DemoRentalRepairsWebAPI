using System;
using Demo.RentalRepairs.Domain.Entities;

namespace Demo.RentalRepairs.Domain.ValueObjects.Request
{
    public class ServiceWorkOrder : TenantRequestBaseDoc
    {
        public Guid  WorkerId { get; set;  }
        public DateTime ServiceDate { get; set;  }
        public int WorkOrderNo { get; set;  }
        public PersonContactInfo Person { get; set;  }

        public ServiceWorkOrder()
        {

        }

        public ServiceWorkOrder(Worker worker, DateTime serviceDate, int workOrderNo)
        {
            WorkerId = worker.Id;
            ServiceDate = serviceDate;
            WorkOrderNo = workOrderNo;
            Person = new PersonContactInfo()
            {
                EmailAddress = worker.PersonContactInfo.EmailAddress,
                FirstName = worker.PersonContactInfo.FirstName,
                LastName = worker.PersonContactInfo.LastName,
                MobilePhone = worker.PersonContactInfo.MobilePhone
            };

        }
        
    }
}
