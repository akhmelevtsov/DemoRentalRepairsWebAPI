using System;

namespace Demo.RentalRepairs.Domain.ValueObjects.Request
{
    public class ServiceWorkOrder : TenantRequestBaseDoc
    {
        public Guid  WorkerId { get; set;  }
        public DateTime ServiceDate { get; set;  }
        public int WorkOrderNo { get; set;  }
        public PersonContactInfo Person { get; set;  }
        
    }
}
