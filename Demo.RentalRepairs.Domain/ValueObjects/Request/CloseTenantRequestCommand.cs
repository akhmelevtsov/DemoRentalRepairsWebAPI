using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.RentalRepairs.Domain.ValueObjects.Request
{
    public class CloseTenantRequestCommand : ITenantRequestCommand 
    {       
        public string Comments()
        {
            return "";
        }
    }
}
    