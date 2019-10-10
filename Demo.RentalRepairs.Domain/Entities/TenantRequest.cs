using System;
using System.Collections.Generic;
using System.Linq;
using Demo.RentalRepairs.Domain.Framework;
using Demo.RentalRepairs.Domain.ValueObjects.Request;

namespace Demo.RentalRepairs.Domain.Entities
{
    public class TenantRequest : Entity
    {
        private TenantRequestStatusEnum _requestStatus = TenantRequestStatusEnum.Undefined;
       
      
        private readonly List<(TenantRequestActionEnum action, TenantRequestStatusEnum next, TenantRequestStatusEnum prev) > _list = new
            List<(TenantRequestActionEnum, TenantRequestStatusEnum, TenantRequestStatusEnum)>
            {
                (TenantRequestActionEnum.RegisterRequest,  TenantRequestStatusEnum.RequestReceived , TenantRequestStatusEnum.Undefined  ),
                (TenantRequestActionEnum.RejectRequest ,TenantRequestStatusEnum.RequestRejected, TenantRequestStatusEnum.RequestReceived),
                (TenantRequestActionEnum.ScheduleWork ,TenantRequestStatusEnum.WorkScheduled, TenantRequestStatusEnum.RequestReceived),
                (TenantRequestActionEnum.ReportWorkIncomplete ,TenantRequestStatusEnum.WorkIncomplete , TenantRequestStatusEnum.WorkScheduled),
                (TenantRequestActionEnum.ReportWorkComplete ,TenantRequestStatusEnum.WorkCompleted , TenantRequestStatusEnum.WorkScheduled),
                (TenantRequestActionEnum.ScheduleWork ,TenantRequestStatusEnum.WorkScheduled, TenantRequestStatusEnum.WorkIncomplete ),
                (TenantRequestActionEnum.CloseRequest ,TenantRequestStatusEnum.Closed, TenantRequestStatusEnum.WorkCompleted ),
                (TenantRequestActionEnum.CloseRequest,TenantRequestStatusEnum.Closed, TenantRequestStatusEnum.RequestRejected )
            };


        public  TenantRequestStatusEnum RequestStatus
        {
            get => _requestStatus;
            private set
            {
                var found = _list.Count(x => x.next == value && x.prev == _requestStatus);
                if (found <= 0)
                    throw new DomainException("domain_error", $"Attempt to change status [{_requestStatus}] to status: [{value}]");

                _requestStatus = value;

            }
        }

        public Tenant Tenant { get; set;  }
        public TenantRequestDoc RequestDoc { get; set; }
        public TenantRequestRejectNotes RejectNotes { get; set; }
        public ServiceWorkReport WorkReport { get; set; }
        public ServiceWorkOrder ServiceWorkOrder { get; set; }
        public int ServiceWorkOrderCount { get; set; }
        public string Code { get; set; }

        private void RegisterRequest(TenantRequestDoc tenantRequestDoc)
        {
            
            RequestStatus = TenantRequestStatusEnum.RequestReceived;
            RequestDoc = tenantRequestDoc;
           
        }

        private void RejectRequest(TenantRequestRejectNotes rejectNotes)
        {
            RequestStatus = TenantRequestStatusEnum.RequestRejected;
            RejectNotes = rejectNotes;
            
        }

        private void ScheduleWork(ServiceWorkOrder serviceWorkOrder)
        {
            RequestStatus = TenantRequestStatusEnum.WorkScheduled ;
            ServiceWorkOrder = serviceWorkOrder;
           
        }

        private void ReportWorkIncomplete(ServiceWorkReport serviceWorkReport)
        {
            RequestStatus = TenantRequestStatusEnum.WorkIncomplete;
            WorkReport = serviceWorkReport;
           
        }

        private void ReportWorkComplete(ServiceWorkReport serviceWorkReport)
        {
            RequestStatus = TenantRequestStatusEnum.WorkCompleted ;
            WorkReport = serviceWorkReport;
        }

        private void CloseRequest()
        {
            RequestStatus = TenantRequestStatusEnum.Closed;
        }

        public TenantRequest ChangeStatus(TenantRequestStatusEnum newStatus, TenantRequestBaseDoc tenantRequestBaseDoc)
        {
            switch (newStatus)
            {

                case TenantRequestStatusEnum.RequestReceived:
                    RegisterRequest((TenantRequestDoc) tenantRequestBaseDoc);
                    break;
                case TenantRequestStatusEnum.RequestRejected:
                    RejectRequest((TenantRequestRejectNotes)tenantRequestBaseDoc);
                    break;
                case TenantRequestStatusEnum.WorkScheduled:
                    ScheduleWork((ServiceWorkOrder)tenantRequestBaseDoc);
                    break;
                case TenantRequestStatusEnum.WorkCompleted:
                    ReportWorkComplete((ServiceWorkReport)tenantRequestBaseDoc);
                    break;
                case TenantRequestStatusEnum.WorkIncomplete:
                    ReportWorkIncomplete((ServiceWorkReport)tenantRequestBaseDoc);
                    break;
                case TenantRequestStatusEnum.Closed:
                    CloseRequest();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newStatus), newStatus, null);
            }
            return this;
        }
        public static void DuplicateException(string requestCode, string unitNumber, string propertyCode)
        {
            throw new DomainException("duplicate_tenant_request", $"Tenant request {requestCode} already exists for unit [{unitNumber}] in [{propertyCode}]");
        }

        public static void NotFoundException(string requestCode, string propertyUnit, string propertyCode)
        {
            throw new DomainEntityNotFoundException("tenant_request_not_found", $"Tenant request [{requestCode}] not found for {propertyCode}/{propertyUnit}");
        }
    }
}
