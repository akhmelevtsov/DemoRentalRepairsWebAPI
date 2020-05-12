using System.Collections.Generic;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.Domain.ValueObjects.Request;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Demo.RentalRepairs.Infrastructure.Mocks
{
    public class NotifyPartiesServiceMock : INotifyPartiesService
    {
     

        public EmailInfo CreateAndSendEmail(TenantRequest tenantRequest)
        {
            throw new System.NotImplementedException();
        }
    }
}
