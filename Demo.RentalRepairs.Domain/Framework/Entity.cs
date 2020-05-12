using System;
using Demo.RentalRepairs.Domain.Services;

namespace Demo.RentalRepairs.Domain.Framework
{
    public abstract class Entity
    {
        
     
        public  Guid Id { get; private set; }
        public DateTime LastUpdated { get; set;  }
        public DateTime DateCreated { get;  set; }

       
        protected Entity(  DateTime? dateCreated = null, Guid? id = null )
        {
            
            this.DateCreated = dateCreated ?? PropertyDomainService.DateTimeProvider.GetDateTime();
            this.Id = id ?? Guid.NewGuid();
        }

        

        
    }
}