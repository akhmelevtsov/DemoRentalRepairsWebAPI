using System;
using System.Collections.Generic;
using System.Text;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.Infrastructure.Repositories.EF.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Demo.RentalRepairs.Infrastructure.Repositories.MongoDb.Models
{
    public class PropertyModel
    {
        [BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }  // Code
        public string Name { get; set; }


        public PropertyAddress Address { get; set; }
        public string PhoneNumber { get; set; }

        public PersonContactInfo Superintendent { get; set; }
        public string NoReplyEmailAddress { get; set; }
        public string LoginEmail { get; set; }


        public List<string> Units { get;  set; }

        public List<TenantModel> Tenants { get; set; } = new List<TenantModel>();
     
        public DateTime DateCreated { get; set; }
        public Guid IdCreated { get; set; }
    }
}
