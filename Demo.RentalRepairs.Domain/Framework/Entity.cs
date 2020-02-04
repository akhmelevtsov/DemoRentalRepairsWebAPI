using System;
using Demo.RentalRepairs.Domain.Services;

namespace Demo.RentalRepairs.Domain.Framework
{
    public abstract class Entity
    {
        public  Guid Id { get; private set; }
        public DateTime LastUpdated { get; set;  }
        public DateTime DateCreated { get;  set; }

        public Entity()
        {
            
        }
        protected Entity( IDateTimeProvider dateTimeProvider)
        {
            this.DateCreated = dateTimeProvider.GetDateTime();
            this.Id = Guid.NewGuid();
        }

        public void UpdateCreateInfo (DateTime created, Guid guid)
        {
            Id = guid;
            DateCreated = created;
        }
        
    }
}