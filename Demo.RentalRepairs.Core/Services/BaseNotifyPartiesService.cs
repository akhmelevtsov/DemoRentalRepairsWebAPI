using System;
using System.Collections.Generic;
using System.Text;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.Exceptions;

namespace Demo.RentalRepairs.Core.Services
{
    public abstract class BaseNotifyPartiesService
    {
        private readonly IPropertyRepository _propertyRepository;

        protected BaseNotifyPartiesService(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }
        protected Worker GetWorkerDetails(TenantRequest tenantRequest)
        {
            Worker worker = null;
            if (tenantRequest.RequestStatus == TenantRequestStatusEnum.Scheduled
                || tenantRequest.RequestStatus == TenantRequestStatusEnum.Done
                || tenantRequest.RequestStatus == TenantRequestStatusEnum.Failed)
            {
                worker = _propertyRepository.GetWorkerByEmail(tenantRequest.WorkerEmail);
                if (worker == null)
                    throw new DomainEntityNotFoundException("worker_not_found", "Worker not found");
            }

            return worker;
        }
    }
}
