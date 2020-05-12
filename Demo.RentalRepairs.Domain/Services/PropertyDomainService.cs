using System;
using System.Collections.Generic;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.Entities.Extensions;
using Demo.RentalRepairs.Domain.Framework;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.Domain.ValueObjects.Request;

namespace Demo.RentalRepairs.Domain.Services
{
    public class PropertyDomainService
    {
        public static  IDateTimeProvider DateTimeProvider { get;  set; } = new DateTimeProvider();



        //public PropertyDomainService(IDateTimeProvider dateTimeProvider = null)
        //{

        //    DateTimeProvider = dateTimeProvider ?? new DateTimeProvider();
        //}

     

    
    }
}
