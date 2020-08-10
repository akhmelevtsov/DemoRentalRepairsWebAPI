using System;
using System.Collections.Generic;
using System.Text;
using Demo.RentalRepairs.Domain.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Demo.RentalRepairs.Infrastructure.Repositories.MongoDb.Models
{
    public class WorkerModel
    {
        [BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        public Guid Id { get; set; }

        public PersonContactInfo ContactInfo { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
