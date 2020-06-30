using System;
using System.Collections.Generic;
using System.Linq;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.Exceptions;
using Demo.RentalRepairs.Domain.Framework;
using Demo.RentalRepairs.Domain.ValueObjects.Request;

namespace Demo.RentalRepairs.Domain.Entities
{
    public class TenantRequest : Entity
    {
        private TenantRequestStatusEnum _requestStatus = TenantRequestStatusEnum.Undefined;


        private readonly
            List<(TenantRequestActionEnum action, TenantRequestStatusEnum next, TenantRequestStatusEnum prev)> _list =
                new
                    List<(TenantRequestActionEnum, TenantRequestStatusEnum, TenantRequestStatusEnum)>
                    {
                        (TenantRequestActionEnum.RegisterRequest, TenantRequestStatusEnum.RequestReceived,
                            TenantRequestStatusEnum.Undefined),
                        (TenantRequestActionEnum.RejectRequest, TenantRequestStatusEnum.RequestRejected,
                            TenantRequestStatusEnum.RequestReceived),
                        (TenantRequestActionEnum.ScheduleWork, TenantRequestStatusEnum.WorkScheduled,
                            TenantRequestStatusEnum.RequestReceived),
                        (TenantRequestActionEnum.ReportWorkIncomplete, TenantRequestStatusEnum.WorkIncomplete,
                            TenantRequestStatusEnum.WorkScheduled),
                        (TenantRequestActionEnum.ReportWorkComplete, TenantRequestStatusEnum.WorkCompleted,
                            TenantRequestStatusEnum.WorkScheduled),
                        (TenantRequestActionEnum.ScheduleWork, TenantRequestStatusEnum.WorkScheduled,
                            TenantRequestStatusEnum.WorkIncomplete),
                        (TenantRequestActionEnum.CloseRequest, TenantRequestStatusEnum.Closed,
                            TenantRequestStatusEnum.WorkCompleted),
                        (TenantRequestActionEnum.CloseRequest, TenantRequestStatusEnum.Closed,
                            TenantRequestStatusEnum.RequestRejected)
                    };


        public TenantRequestStatusEnum RequestStatus
        {
            get => _requestStatus;
            private set
            {
                var found = _list.Count(x => x.next == value && x.prev == _requestStatus);
                if (found <= 0)
                    throw new DomainException("domain_error",
                        $"Attempt to change status [{_requestStatus}] to status: [{value}]");

                _requestStatus = value;

            }
        }

        public Tenant Tenant { get; set; }
        public TenantRequestDoc RequestDoc { get; set; }
        public TenantRequestRejectNotes RejectNotes { get; set; }
        public ServiceWorkReport WorkReport { get; set; }
        public ServiceWorkOrder ServiceWorkOrder { get; set; }
        public int ServiceWorkOrderCount { get; set; }
        public string Code { get; set; }
        public string WorkerEmail { get; set; }

        

        public TenantRequest(Tenant tenant, string code, DateTime? dateCreated = null, Guid? id = null) : base(
            dateCreated, id)
        {
            Tenant = tenant;
            Code = code;
        }

        public TenantRequest(Tenant tenant, string code, TenantRequestStatusEnum requestStatus, DateTime dateCreated,
            Guid idGuid) : this(tenant, code, dateCreated, idGuid)
        {
            _requestStatus = requestStatus;
        }

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
            RequestStatus = TenantRequestStatusEnum.WorkScheduled;
            ServiceWorkOrder = serviceWorkOrder;
            this.WorkerEmail = serviceWorkOrder.Person.EmailAddress;

        }

        private void ReportWorkIncomplete(ServiceWorkReport serviceWorkReport)
        {
            RequestStatus = TenantRequestStatusEnum.WorkIncomplete;
            WorkReport = serviceWorkReport;

        }

        private void ReportWorkComplete(ServiceWorkReport serviceWorkReport)
        {
            RequestStatus = TenantRequestStatusEnum.WorkCompleted;
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
                    RejectRequest((TenantRequestRejectNotes) tenantRequestBaseDoc);
                    break;
                case TenantRequestStatusEnum.WorkScheduled:
                    ScheduleWork((ServiceWorkOrder) tenantRequestBaseDoc);
                    break;
                case TenantRequestStatusEnum.WorkCompleted:
                    ReportWorkComplete((ServiceWorkReport) tenantRequestBaseDoc);
                    break;
                case TenantRequestStatusEnum.WorkIncomplete:
                    ReportWorkIncomplete((ServiceWorkReport) tenantRequestBaseDoc);
                    break;
                case TenantRequestStatusEnum.Closed:
                    CloseRequest();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newStatus), newStatus, null);
            }

            return this;
        }

        public string TenantFullName => Tenant?.ContactInfo?.GetFullName();
        public string RequestDate => DateCreated.ToLongTimeString();
        public string PropertyName => Tenant?.Property?.Name;
        public string WorkerFullName => ServiceWorkOrder?.Person?.GetFullName();
        public string SuperintendentFullName => Tenant?.Property?.Superintendent?.GetFullName();
        public string WorkOrderNumber => ServiceWorkOrder?.WorkOrderNo.ToString();
        public string PropertyId => Tenant?.Property?.Id.ToString();
        public string TenantId => Tenant?.Id.ToString();
        public string WorkerId => ServiceWorkOrder?.Person?.EmailAddress;
        public string PropertyNoReplyEmail => Tenant?.Property?.NoReplyEmailAddress;
        public string TenantEmail => Tenant?.ContactInfo?.EmailAddress;
        public string PropertyPhone => Tenant?.Property?.PhoneNumber;
        public string SuperintendentEmail => Tenant?.Property?.Superintendent?.EmailAddress;
        public string WorkOrderDate => ServiceWorkOrder?.ServiceDate.ToShortDateString() ;
    }
}
