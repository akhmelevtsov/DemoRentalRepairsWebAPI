using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.Exceptions;
using Demo.RentalRepairs.Domain.Framework;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.Domain.ValueObjects.Request;

namespace Demo.RentalRepairs.Domain.Entities
{
    public class TenantRequest : Entity
    {
        readonly DomainValidationService _validationService = new DomainValidationService();

        private TenantRequestStatusEnum _requestStatus = TenantRequestStatusEnum.Undefined;


        private readonly
            List<( TenantRequestStatusEnum next, TenantRequestStatusEnum prev)> _list =
                new
                    List<( TenantRequestStatusEnum, TenantRequestStatusEnum)>
                    {
                        ( TenantRequestStatusEnum.Submitted,TenantRequestStatusEnum.Undefined),
                        (TenantRequestStatusEnum.Declined,TenantRequestStatusEnum.Submitted),
                        (TenantRequestStatusEnum.Scheduled,TenantRequestStatusEnum.Submitted),
                        ( TenantRequestStatusEnum.Failed,TenantRequestStatusEnum.Scheduled),
                        (TenantRequestStatusEnum.Done,TenantRequestStatusEnum.Scheduled),
                        (TenantRequestStatusEnum.Scheduled,TenantRequestStatusEnum.Failed),
                        (TenantRequestStatusEnum.Closed,TenantRequestStatusEnum.Done),
                        (TenantRequestStatusEnum.Closed,TenantRequestStatusEnum.Declined)
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
 
        public int ServiceWorkOrderCount { get; set; }
        public string Code { get; set; }
       
        public List<TenantRequestChange> RequestChanges { get; set; } = new List<TenantRequestChange>();


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

        public TenantRequest ExecuteCommand(ITenantRequestCommand command)
        {
            var type = command.GetType();
            if (type == typeof(RegisterTenantRequestCommand))
            {
                _validationService.Validate((RegisterTenantRequestCommand)command);
                RequestStatus = TenantRequestStatusEnum.Submitted;
                RequestChanges.Add(new TenantRequestChange(RequestStatus, command));
                return this;
            }

            if (type == typeof(RejectTenantRequestCommand))
            {
               
                RequestStatus = TenantRequestStatusEnum.Declined;
                RequestChanges.Add(new TenantRequestChange(RequestStatus, command));
                return this;
            }

            if (type == typeof(ScheduleServiceWorkCommand))
            {
                _validationService.Validate((ScheduleServiceWorkCommand)command);
                RequestStatus = TenantRequestStatusEnum.Scheduled;
                ServiceWorkOrderCount++;
                RequestChanges.Add(new TenantRequestChange(RequestStatus, command,ServiceWorkOrderCount ));
               return this;
            }

            if (type == typeof(ReportServiceWorkCommand)  && ((ReportServiceWorkCommand)command).Success)
            {
                _validationService.Validate((ReportServiceWorkCommand)command);
                RequestStatus = TenantRequestStatusEnum.Done;
                RequestChanges.Add(new TenantRequestChange(RequestStatus, command, ServiceWorkOrderCount));

                return this;
            }
            if (type == typeof(ReportServiceWorkCommand) && !((ReportServiceWorkCommand)command).Success)
            {
                _validationService.Validate((ReportServiceWorkCommand)command);
                RequestStatus = TenantRequestStatusEnum.Failed;
                RequestChanges.Add(new TenantRequestChange(RequestStatus, command, ServiceWorkOrderCount));

                return this;
            }
            if (type == typeof(CloseTenantRequestCommand) )
            {
                RequestStatus = TenantRequestStatusEnum.Closed ;
                RequestChanges.Add(new TenantRequestChange(RequestStatus, command));

                return this;
            }
            throw new ArgumentOutOfRangeException(nameof(command));
        }

        public string TenantFullName => Tenant?.ContactInfo?.GetFullName();
        public string RequestDate => DateCreated.ToShortDateString();
        public string RequestTitle => GetRegisterCommand().Title;
        public string RequestDescription => GetRegisterCommand().Description ;
        public string PropertyName => Tenant?.Property?.Name;
        public string SuperintendentFullName => Tenant?.Property?.Superintendent?.GetFullName();

        public string WorkOrderNumber => GetScheduleWorkCommand()?.WorkOrderNo.ToString();
        public string WorkOrderDate => GetScheduleWorkCommand()?.ServiceDate.ToShortDateString();
        public string WorkerEmail => GetScheduleWorkCommand()?.WorkerEmailAddress ;

        public string PropertyId => Tenant?.Property?.Id.ToString();
        public string TenantId => Tenant?.Id.ToString();
        public string TenantUnit => Tenant?.UnitNumber;
    
        public string PropertyNoReplyEmail => Tenant?.Property?.NoReplyEmailAddress;
        public string TenantEmail => Tenant?.ContactInfo?.EmailAddress;
        public string PropertyPhone => Tenant?.Property?.PhoneNumber;
        public string SuperintendentEmail => Tenant?.Property?.Superintendent?.EmailAddress;

        private RegisterTenantRequestCommand GetRegisterCommand() => RequestChanges.OrderBy(x => x.DateCreated)
            .Where(x => x.TenantRequestStatus == TenantRequestStatusEnum.Submitted)
            .Select(x => (RegisterTenantRequestCommand) x.Command).First();
        private ScheduleServiceWorkCommand GetScheduleWorkCommand()
        {
            var tenantRequestChanges = RequestChanges.OrderBy(x => x.DateCreated)
                .Where(x => x.TenantRequestStatus == TenantRequestStatusEnum.Scheduled && x.Num == ServiceWorkOrderCount);

            var requestChanges = tenantRequestChanges.ToList();
            return  requestChanges.Any() ?  requestChanges.Select(x => (ScheduleServiceWorkCommand) x.Command).Last() : null ;
        }

   
    }
}
