using System;
using Demo.RentalRepairs.Domain.Services;

namespace Demo.RentalRepairs.Domain.Framework
{
    public abstract class Entity
    {
        public  Guid Id { get; private set; }
        public DateTime LastUpdated { get; set;  }
        public DateTime DateCreated { get;  set; }

       
        protected Entity( IDateTimeProvider dateTimeProvider, DateTime? dateCreated = null, Guid? id = null )
        {
            this.DateCreated = dateCreated ?? dateTimeProvider.GetDateTime();
            this.Id = id ?? Guid.NewGuid();
        }

        public void UpdateCreateInfo (DateTime created, Guid guid)
        {
            Id = guid;
            DateCreated = created;
        }
        
    }
}