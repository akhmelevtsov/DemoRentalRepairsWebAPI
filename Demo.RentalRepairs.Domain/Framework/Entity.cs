using System;

namespace Demo.RentalRepairs.Domain.Framework
{
    public abstract class Entity
    {
        public  Guid Id { get; private set; }
        public DateTime LastUpdated { get; set;  }
        public DateTime DateCreated { get;  set; }

        protected Entity()
        {
            this.Id = Guid.NewGuid();
        }
        
    }
}