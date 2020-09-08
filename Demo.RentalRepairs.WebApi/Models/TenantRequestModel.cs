using System;
using System.ComponentModel;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.ValueObjects.Request;

namespace Demo.RentalRepairs.WebApi.Models
{
    public class TenantRequestModel
    {
        [DisplayName("N")]
        public string RequestCode { get; set;  }
        [DisplayName("Status")]
        public string RequestStatus { get; set;  }
        [DisplayName("Created")]
        public DateTime DateCreated { get; set;  }
      
        public string RequestTitle { get; set; }
     
       
    }
}
