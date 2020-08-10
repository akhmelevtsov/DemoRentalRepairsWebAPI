using System;
using System.Collections.Generic;
using System.Text;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.Infrastructure.Repositories.EF.Entities;
using MongoDB.Bson.Serialization.Attributes;

namespace Demo.RentalRepairs.Infrastructure.Repositories.MongoDb.Models
{
    public class TenantModel
    {
        public Guid Id { get; set; }
        [BsonIgnore]
        public PropertyModel PropertyModel { get; set; }
        public PersonContactInfo ContactInfo { get; set; }
        public string PropertyTblID { get; set; }  //Code
      
        public string UnitNumber { get; set; }

        public List<TenantRequestModel> Requests { get; set; } = new List<TenantRequestModel>();
        public DateTime DateCreated { get; set; }

    }
}
