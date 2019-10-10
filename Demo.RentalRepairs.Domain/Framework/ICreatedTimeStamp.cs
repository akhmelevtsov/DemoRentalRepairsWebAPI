using System;

namespace Demo.RentalRepairs.Domain.Framework
{
    public interface ICreatedTimeStamp
    {
        DateTime DateCreated { get; set; }
        Guid CreatorId { get; set; }
    }
}
