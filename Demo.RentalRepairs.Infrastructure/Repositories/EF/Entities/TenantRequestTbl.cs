﻿using System;
using System.ComponentModel.DataAnnotations.Schema;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.ValueObjects.Request;

namespace Demo.RentalRepairs.Infrastructure.Repositories.EF.Entities
{
    public class TenantRequestTbl
    {
        public Guid ID { get; set; }
        public TenantTbl Tenant { get; set; }
         public Guid TenantID { get; set; }
        public TenantRequestStatusEnum RequestStatus { get; set; }
        public string RequestDoc { get; set; }          //TenantRequestDoc
        public string RejectNotes { get; set; }         //TenantRequestRejectNotes
     
        public string WorkReport { get; set; }          //ServiceWorkReport
       
        public string ServiceWorkOrder { get; set; }    //ServiceWorkOrder
        public int ServiceWorkOrderCount { get; set; }
        public string Code { get; set; }
        public DateTime DateCreated { get; set; }
    }
}