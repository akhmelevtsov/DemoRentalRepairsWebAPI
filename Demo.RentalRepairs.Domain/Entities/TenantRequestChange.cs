using System;
using System.Collections.Generic;
using System.Text;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.Framework;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.Domain.ValueObjects.Request;

namespace Demo.RentalRepairs.Domain.Entities
{
    public class TenantRequestChange : Entity
    {
        public TenantRequestStatusEnum TenantRequestStatus { get; }
        public ITenantRequestCommand Command { get; }
        public int Num { get; }

        public TenantRequestChange(TenantRequestStatusEnum tenantRequestStatus, ITenantRequestCommand command, int num =1,  DateTime? dateCreated= null, Guid? id = null) : base(dateCreated, id)
        {
            TenantRequestStatus = tenantRequestStatus;
            Command = command;
            Num = num;
        }

        public string Comments()
        {
            return Command.Comments();
        }
    }
}
