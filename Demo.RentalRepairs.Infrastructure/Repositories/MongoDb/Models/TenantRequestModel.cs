using System;
using System.Collections.Generic;
using System.Text;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace Demo.RentalRepairs.Infrastructure.Repositories.MongoDb.Models
{
    public class TenantRequestModel
    {
        public Guid ID { get; set; }
        [BsonIgnore]
        public TenantModel TenantModel { get; set;  }
        public Guid TenantID { get; set; }
        public TenantRequestStatusEnum RequestStatus { get; set; }

        public int ServiceWorkOrderCount { get; set; }
        public string Code { get; set; }
        public string RequestChanges { get; set; } // = new List<TenantRequestEventModel>( );
        public DateTime DateCreated { get; set; }
       
    }
}
